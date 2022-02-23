using System.Reflection;
using AutoApi.Descriptive;
using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Exceptions;
using AutoApi.QueryRequestDefinition;
using AutoApi.QueryRequestDefinition.Parameters;
using AutoApi.QueryRequestDefinitions.Parameters;
using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Configuration.Wire;
using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Responses;
using AutoApi.Toolkit;

namespace AutoApi.Rest.Configuration.Builders;

public static class EntitySettingsBuilder
{
    public static AutoApiRestConfigurationOptions Build(this AutoApiRestConfigurationOptions config)
    {
        var exportedTypes = config.ContextTypes.SelectMany(x => x.Assembly.GetExportedTypes()).Distinct().ToArray();

        PerformAttributesChecking(exportedTypes);

        var entityTypes = GetEntityTypes(exportedTypes).ToArray();
        var collectiveTypes = GetEntitiesCollectiveTypes(exportedTypes).ToArray();
        var parameters = GetAllParameters(collectiveTypes);
        var fieldDescriptions = GetFieldDescriptions(collectiveTypes);

        var responseTypes = collectiveTypes.Where(x => x.GetInterface(nameof(IEntityResponse)) is not null).ToArray();
        var queryTypes = collectiveTypes.Where(x => x.GetInterface(nameof(IGetRequest)) is not null).ToArray();
        var commandTypes = collectiveTypes.Where(x => x.GetInterface(nameof(ICommandRequest)) is not null).ToArray();

        var entitySettingsCollection = CreateEntitySettingsCollection(
            config,
            exportedTypes,
            collectiveTypes,
            parameters,
            fieldDescriptions,
            entityTypes,
            responseTypes,
            queryTypes,
            commandTypes).ToArray();

        config.ExtendedRepositoryType = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));

        config.EntitySettingsCollection = entitySettingsCollection;

        return config;
    }

    private static IEnumerable<Type> GetForeignRelations(Type entityType, Type[] allEntityTypes)
    {
        var entityTypes = allEntityTypes.Select(x => typeof(ICollection<>).MakeGenericType(x)).ToArray();

        foreach (var prop in entityType.GetProperties())
        {
            if (entityTypes.Contains(prop.PropertyType))
            {
                yield return prop.PropertyType;
            }
        }
    }

    private static IEnumerable<EntitySettings> CreateEntitySettingsCollection(
        AutoApiRestConfigurationOptions config,
        Type[] exportedTypes,
        Type[] collectiveTypes,
        IDictionary<Type, IEnumerable<ConfigurationDefinition.Parameter>> parameters,
        IDictionary<Type, IEnumerable<ConfigurationDefinition.Field>> fields,
        Type[] entityTypes,
        Type[] responseTypes,
        Type[] queryTypes,
        Type[] commandTypes)
    {
        var entityDefinitions = entityTypes.Select(entityType => new ConfigurationDefinition.Entity()
        {
            EntityType = entityType,
            ResponseType = responseTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name))!,
            QueryType = queryTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name))!,
            CommandType = commandTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name))!,
            ForeignCollectionTypes = GetForeignRelations(entityType, entityTypes).ToArray(),
        }).ToArray();

        foreach (var entityDefinition in entityDefinitions)
        {
            if (!config.EndpointSettingsCollection.TryGetValue(entityDefinition.EntityType, out var controllerEndpoint)) continue;

            var foreignCollectionEntities = entityDefinitions
                .Except(new[] { entityDefinition })
                .Where(x => entityDefinition.ForeignCollectionTypes.Contains(typeof(ICollection<>).MakeGenericType(x.EntityType)))
                .ToArray();

            var controllerType = DynamicAssemblyBuilder.CreateDynamicControllerType(entityDefinition, foreignCollectionEntities);

            yield return new EntitySettings
            {
                EntityType = entityDefinition.EntityType,
                ResponseType = entityDefinition.ResponseType,
                QueryRequestType = entityDefinition.QueryType,
                CommandRequestType = entityDefinition.CommandType,
                QueryConfigurationType = collectiveTypes.GetFirstInterfaceChild(typeof(IQueryConfiguration), entityDefinition.EntityType.Name)!,
                ControllerRoute = controllerEndpoint.Route,
                ControllerName = controllerType.AssemblyQualifiedName!,
                ParameterConfigurations = parameters.FirstOrDefault(x => x.Key == entityDefinition.QueryType).Value,
                //ValidatorType = GetValidator(exportedTypes, entityDefinition.CommandType),
                AuthorizeableEndpoints = GetAuthorizeableEndpoints(controllerEndpoint, controllerType),
                AutoExpandMembers = controllerEndpoint.AutoExpandMembers,
                ExplicitExpandedMembers = controllerEndpoint.ExplicitExpandedMembers!,
                FieldDescriptions = fields.FirstOrDefault(x => x.Key == entityDefinition.ResponseType),
                ForeignEntityDefinitions = foreignCollectionEntities,
            };
        }
    }

    private static IDictionary<Type, IEnumerable<ConfigurationDefinition.Field>> GetFieldDescriptions(IEnumerable<Type> types)
    {
        var responses = types.Where(x => x.BaseType == typeof(EntityResponse));
        var fieldCollection = new Dictionary<Type, IEnumerable<ConfigurationDefinition.Field>>();
        foreach (var response in responses)
        {
            var properties = response.GetProperties()
                .Select(x => (x.Name, x.PropertyType.GetTypeInfo()));

            var fields = properties
                .Where(x => x.Item2.GenericTypeArguments != Array.Empty<Type>())
                .Select(x => (x, x.Item2.GetGenericArguments()[0]))
                .Where(x => x.Item2.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISimplified)))
                .Select(x => new ConfigurationDefinition.Field(x.x.Name, ConfigurationDefinition.FieldCategory.Relational))
                .ToList();

            var defaultFields = properties
                .Where(y => fields.All(x => !y.Name.Equals(x.Name)))
                .Select(x => new ConfigurationDefinition.Field(x.Name, ConfigurationDefinition.FieldCategory.Default));

            fields.AddRange(defaultFields);
            fieldCollection.Add(response, fields);
        }

        return fieldCollection;
    }

    private static void PerformAttributesChecking(Type[] exportedTypes)
    {
        var requests = exportedTypes
            .Where(x => x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICommandRequest)));

        var idCollectionAttributes = requests
            .SelectMany(x => x.GetProperties())
            .Select(x => x.GetCustomAttribute<IdCollectionAttribute>())
            .Where(x => x is not null);

        // Todo: check query parameter attribute

        if (idCollectionAttributes is not null && !idCollectionAttributes.Any(x => x!.EntityType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity))))
        {
            throw new InvalidConfigurationException(ErrorMessage.Configuration.IdCollectionAttribute);
        }
    }

    private static IDictionary<Type, IEnumerable<ConfigurationDefinition.Parameter>> GetAllParameters(IEnumerable<Type> collectedTypes)
    {
        //todo use interface
        var requestTypes = collectedTypes.Where(x => x.BaseType == typeof(GetRequest));

        var parameterDescriptionsCollection = new Dictionary<Type, IEnumerable<ConfigurationDefinition.Parameter>>();
        foreach (var requestType in requestTypes)
        {
            var parameterDescriptions = new List<ConfigurationDefinition.Parameter>()
            {
                new(typeof(CreatedDateParameter), nameof(IGetRequest.CreatedDate), new[] { nameof(IEntity.CreatedDate) }),
                new(typeof(UpdatedDateParameter), nameof(IGetRequest.UpdatedDate), new[] { nameof(IEntity.UpdatedDate) })
            };

            parameterDescriptions.AddRange(CollectParameterDescriptions(requestType, parameterDescriptions));

            parameterDescriptionsCollection.Add(requestType, parameterDescriptions);
        }

        return parameterDescriptionsCollection;
    }

    private static IEnumerable<ConfigurationDefinition.Parameter> CollectParameterDescriptions(
        Type requestType,
        List<ConfigurationDefinition.Parameter> parameterDescriptions)
    {
        foreach (var property in requestType.GetProperties())
        {
            var attribute = property.GetCustomAttribute<QueryParameterAttribute>();

            if (attribute is null) continue;

            yield return new(attribute.ParameterType, property.Name, attribute.NavigationNodes ?? new[] { property.Name });
        }
    }

    private static IEnumerable<Type> GetEntitiesCollectiveTypes(Type[] exportedTypes)
    {
        return exportedTypes.Where(x =>
            !x.IsAbstract &&
            x.BaseType == typeof(EntityResponse) ||
            x.BaseType == typeof(GetRequest) ||
            x.GetInterface(nameof(ICommandRequest)) != null ||
            x.GetInterface(nameof(IQueryConfiguration)) != null); // move this last piece?
    }

    private static IOrderedEnumerable<Type> GetEntityTypes(Type[] exportedTypes)
    {
        return exportedTypes.Where(x =>
             !x.IsAbstract &&
             !x.IsInterface &&
             x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity)))
            .OrderByDescending(x => x.Name.Length);
    }

    //private static Type GetValidator(Type[] exportedTypes, Type commandRequest)
    //{
    //    return exportedTypes
    //        .FirstOrDefault(x => x.IsSubclassOf(typeof(AbstractValidator<>)
    //            .MakeGenericType(commandRequest))) ??
    //        typeof(DefaultValidation<>).MakeGenericType(commandRequest);
    //}

    private static IDictionary<string, AuthorizeableEndpoint> GetAuthorizeableEndpoints(
        ControllerEndpointSettings controllerEndpoint, Type controllerType)
    {
        var authorizeEndpoints = new Dictionary<string, AuthorizeableEndpoint>();

        foreach (var actionEndpoint in controllerEndpoint.ActionSettingsCollection)
        {
            authorizeEndpoints.Add($"{controllerType.FullName}.{actionEndpoint.ActionMethod}", new()
            {
                Category = actionEndpoint?.AuthorizeableEndpoint.Category == AuthorizationCategory.None ?
                controllerEndpoint.AuthorizeableEndpoint.Category : actionEndpoint!.AuthorizeableEndpoint.Category,

                Policy = string.IsNullOrWhiteSpace(actionEndpoint.AuthorizeableEndpoint.Policy) ?
                controllerEndpoint.AuthorizeableEndpoint.Policy : actionEndpoint.AuthorizeableEndpoint.Policy
            });
        }

        return authorizeEndpoints;
    }
}
