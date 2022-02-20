//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Security.Principal;
//using System.Text;
//using System.Threading.Tasks;
//using AutoApi.Rest.Configuration.Definitions;
//using AutoApi.Rest.Configuration.Settings;
//using AutoApi.Rest.Configuration.Wire;
//using AutoApi.Rest.Shared.Requests;
//using AutoApi.Rest.Shared.Responses;
//using AutoApi.Toolkit;

//namespace AutoApi.Rest.Configuration.Service;

//public static class ServiceBuilder
//{
//    public static AutoApiRestConfigurationOptions Build(this AutoApiRestConfigurationOptions options)
//    {
//        var exportedTypes = options.ContextTypes!.SelectMany(x => x.Assembly.GetExportedTypes()).Distinct().ToArray();

//        PerformAttributesChecking(exportedTypes);

//        var entityTypes = GetEntityTypes(exportedTypes).ToArray();
//        var collectiveTypes = GetEntitiesCollectiveTypes(exportedTypes).ToArray();
//        var parameters = GetAllParameters(collectiveTypes);
//        var fieldDescriptions = GetFieldDescriptions(collectiveTypes);

//        var responseTypes = collectiveTypes.Where(x => x.BaseType == typeof(EntityResponse)).ToArray();
//        var queryTypes = collectiveTypes.Where(x => x.BaseType == typeof(GetRequest)).ToArray();
//        var commandTypes = collectiveTypes.Where(x => x.GetInterface(nameof(IModifyRequest)) != null).ToArray();

//        var entitySettingsCollection = CreateEntitySettingsCollection(
//            options,
//            exportedTypes,
//            collectiveTypes,
//            parameters,
//            fieldDescriptions,
//            entityTypes,
//            responseTypes,
//            queryTypes,
//            commandTypes).ToArray();

//        GetRequestType = (entityName) => _requestCollection[entityName];
//        GetResponseType = (entityName) => _responseCollection[entityName];

//        options.ExtendedRepositoryType = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));
//        options.EntitySettingsCollection = entitySettingsCollection;
//        return options;
//    }

//    private static readonly Dictionary<string, Type> _requestCollection = new();
//    private static readonly Dictionary<string, Type> _responseCollection = new();

//    public static Func<string, Type> GetRequestType { get; private set; }
//    public static Func<string, Type> GetResponseType { get; private set; }

//    private static IEnumerable<Type> GetForeignRelations(Type entityType, Type[] allEntityTypes)
//    {
//        var entityTypes = allEntityTypes.Select(x => typeof(ICollection<>).MakeGenericType(x)).ToArray();

//        foreach (var prop in entityType.GetProperties())
//        {
//            if (entityTypes.Contains(prop.PropertyType))
//            {
//                yield return prop.PropertyType;
//            }
//        }
//    }

//    private static IEnumerable<EntitySettings> CreateEntitySettingsCollection(
//        RapierConfigurationOptions config,
//        Type[] exportedTypes,
//        Type[] collectiveTypes,
//        IDictionary<Type, IEnumerable<ParameterConfigurationDescription>> parameters,
//        IDictionary<Type, IEnumerable<FieldDescription>> fieldDescriptions,
//        Type[] entityTypes,
//        Type[] responseTypes,
//        Type[] queryTypes,
//        Type[] commandTypes)
//    {
//        var entityDefinitions = entityTypes.Select(entityType => new EntityDefinition()
//        {
//            EntityType = entityType,
//            ResponseType = responseTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name)),
//            QueryType = queryTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name)),
//            CommandType = commandTypes.FirstOrDefault(y => y.Name.Contains(entityType.Name)),
//            ForeignCollectionTypes = GetForeignRelations(entityType, entityTypes).ToArray(),
//        }).ToArray();

//        foreach (var entityDefinition in entityDefinitions)
//        {
//            if (!config.EndpointSettingsCollection.TryGetValue(entityDefinition.EntityType, out var controllerEndpoint)) continue;

//            _requestCollection.Add(entityDefinition.EntityType.Name, entityDefinition.CommandType);
//            _responseCollection.Add(entityDefinition.EntityType.Name, entityDefinition.ResponseType);

//            var foreignCollectionEntities = entityDefinitions
//                .Except(new[] { entityDefinition })
//                .Where(x => entityDefinition.ForeignCollectionTypes.Contains(typeof(ICollection<>).MakeGenericType(x.EntityType)))
//                .ToArray();

//            var controllerType = DynamicAssemblyBuilder.CreateDynamicControllerType(entityDefinition, foreignCollectionEntities);

//            yield return new EntitySettings
//            {
//                EntityType = entityDefinition.EntityType,
//                ResponseType = entityDefinition.ResponseType,
//                QueryRequestType = entityDefinition.QueryType,
//                CommandRequestType = entityDefinition.CommandType,
//                QueryConfigurationType = collectiveTypes.GetFirstInterfaceChild(typeof(IQueryConfiguration), entityDefinition.EntityType.Name),
//                ControllerRoute = controllerEndpoint.Route,
//                ControllerName = controllerType.AssemblyQualifiedName,
//                ParameterConfigurations = parameters.FirstOrDefault(x => x.Key == entityDefinition.QueryType).Value,
//                ValidatorType = GetValidator(exportedTypes, entityDefinition.CommandType),
//                AuthorizeableEndpoints = GetAuthorizeableEndpoints(controllerEndpoint, controllerType),
//                AutoExpandMembers = controllerEndpoint.AutoExpandMembers,
//                ExplicitExpandedMembers = controllerEndpoint.ExplicitExpandedMembers,
//                FieldDescriptions = fieldDescriptions.FirstOrDefault(x => x.Key == entityDefinition.ResponseType),
//                ForeignEntityDefinitions = foreignCollectionEntities,
//            };
//        }

//    private static IDictionary<Type, IEnumerable<FieldDescription>> GetFieldDescriptions(IEnumerable<Type> types)
//    {
//        var responses = types.Where(x => x.BaseType == typeof(EntityResponse));
//        var fieldCollection = new Dictionary<Type, IEnumerable<FieldDescription>>();
//        foreach (var response in responses)
//        {
//            var properties = response.GetProperties()
//                .Select(x => (x.Name, x.PropertyType.GetTypeInfo()));

//            var fields = properties
//                .Where(x => x.Item2.GenericTypeArguments != Array.Empty<Type>())
//                .Select(x => (x, x.Item2.GetGenericArguments()[0]))
//                .Where(x => x.Item2.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISimplified)))
//                .Select(x => new FieldDescription(x.x.Name, FieldCategory.Relational))
//                .ToList();

//            var defaultFields = properties
//                .Where(y => fields.All(x => !y.Name.Equals(x.Name)))
//                .Select(x => new FieldDescription(x.Name, FieldCategory.Default));

//            fields.AddRange(defaultFields);
//            fieldCollection.Add(response, fields);
//        }

//        return fieldCollection;
//    }

//    private static void PerformAttributesChecking(Type[] exportedTypes)
//    {
//        var requests = exportedTypes
//            .Where(x => x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IModifyRequest)));
//var idCollectionAttributes = requests
//.SelectMany(x => x.GetProperties())
//            .Select(x => x.GetCustomAttribute<IdCollectionAttribute>())
//            .Where(x => x is not null);

//        // Todo: check query parameter attribute

//        if (idCollectionAttributes is not null)
//            if (!idCollectionAttributes
//                .Any(x => x.EntityType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity))))
//                throw new InvalidConfigurationException(ErrorMessage.Configuration.IdCollectionAttribute);
//    }

//    private static IDictionary<Type, IEnumerable<ParameterConfigurationDescription>> GetAllParameters(IEnumerable<Type> collectedTypes)
//    {
//        var requestTypes = collectedTypes.Where(x => x.BaseType == typeof(GetRequest));

//        var parameterDescriptionsCollection = new Dictionary<Type, IEnumerable<ParameterConfigurationDescription>>();
//foreach (var requestType in requestTypes)
//{
//            var parameterDescriptions = new List<ParameterConfigurationDescription>()
//                {
//                    new(typeof(CreatedDateParameter), nameof(GetRequest.CreatedDate), new[] { nameof(IEntity.CreatedDate) }),
//                    new(typeof(UpdatedDateParameter), nameof(GetRequest.UpdatedDate), new[] { nameof(IEntity.UpdatedDate) })
//                };

//            CollectParameterDescriptions(
//                parameterDescriptionsCollection, requestType, parameterDescriptions);
//        }

//        return parameterDescriptionsCollection;
//    }

//    private static void CollectParameterDescriptions(
//        Dictionary<Type, IEnumerable<ParameterConfigurationDescription>> parameterDescriptionsCollection,
//        Type requestType, List<ParameterConfigurationDescription> parameterDescriptions)
//    {
//        foreach (var property in requestType.GetProperties())
//        {
//            var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
//            if (attribute is null)
//                continue;

//            parameterDescriptions.Add(
//                new ParameterConfigurationDescription(
//                    attribute.ParameterType, property.Name, attribute.NavigationNodes ?? new[] { property.Name }));
//        }

//        parameterDescriptionsCollection.Add(requestType, parameterDescriptions);
//    }

//    private static IEnumerable<Type> GetEntitiesCollectiveTypes(Type[] exportedTypes)
//    {
//        return exportedTypes.Where(x =>
//            !x.IsAbstract &&
//            x.BaseType == typeof(EntityResponse) ||
//            x.BaseType == typeof(GetRequest) ||
//            x.GetInterface(nameof(IModifyRequest)) != null ||
//            x.GetInterface(nameof(IQueryConfiguration)) != null); // move this last piece?
//    }

//    private static IOrderedEnumerable<Type> GetEntityTypes(Type[] exportedTypes)
//    {
//        return exportedTypes.Where(x =>
//             !x.IsAbstract &&
//             !x.IsInterface &&
//             x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity)))
//            .OrderByDescending(x => x.Name.Length);
//    }

//    private static Type GetValidator(Type[] exportedTypes, Type commandRequest)
//    {
//        return exportedTypes
//            .FirstOrDefault(x => x.IsSubclassOf(typeof(AbstractValidator<>)
//                .MakeGenericType(commandRequest))) ??
//            typeof(DefaultValidation<>).MakeGenericType(commandRequest);
//    }

//    private static IDictionary<string, AuthorizeableEndpoint> GetAuthorizeableEndpoints(
//        ControllerEndpointSettings controllerEndpoint, Type controllerType)
//    {
//        var authorizeEndpoints = new Dictionary<string, AuthorizeableEndpoint>();
//        foreach (var actionEndpoint in controllerEndpoint.ActionSettingsCollection)
//        {
//            authorizeEndpoints.Add(
//                $"{controllerType.FullName}.{actionEndpoint.ActionMethod}",
//                new()
//                {
//                    Category = actionEndpoint?.AuthorizeableEndpoint.Category == AuthorizationCategory.None ?
//                    controllerEndpoint.AuthorizeableEndpoint.Category : actionEndpoint.AuthorizeableEndpoint.Category,

//                    Policy = string.IsNullOrWhiteSpace(actionEndpoint.AuthorizeableEndpoint.Policy) ?
//                    controllerEndpoint.AuthorizeableEndpoint.Policy : actionEndpoint.AuthorizeableEndpoint.Policy
//                });
//        }

//        return authorizeEndpoints;
//    }
//}
