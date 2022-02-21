using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoApi.EntityFramework.Repository;
using AutoApi.Exceptions;
using AutoApi.Mediator;
using AutoApi.QueryRequestDefinition;
using AutoApi.Rest.Configuration.Builders;
using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.EntityFramework.RequestManagement;
using AutoApi.Rest.Pipeline.Controller;
using AutoApi.Rest.Pipeline.PipelineBehaviours;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.ResponseManagement;
using AutoApi.Toolkit;
using AutoApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoApi.Rest.Configuration.Wire;

public static class ServiceCollectionExtensions
{
    //public static void AddAutoApiControllers(this IServiceCollection services)
    //{
    //    services.AddControllers();
    //}

    //public static void AddMediator(this IServiceCollection services)
    //{
    //    services.AddTransient<ServiceFactory>(p => p.GetService);
    //    services.AddTransient<IMediator, MediatorImpl>();

    //    services.AddScoped<IPipelineBehaviour<TestRequest, TestResponse>, LoggingBehaviour>();
    //    services.AddScoped<IRequestHandler<TestRequest, TestResponse>, GetHandler>();
    //}}


    public static void AddRestControllers(this IServiceCollection services, Action<AutoApiRestConfigurationOptions> options)
    {
        var config = options.Invoke();

        AddEndpoints(config);

        if (config.InterfaceDiscovery && config.ContextTypes is not null)
        {
            config.Build();
        }

        services.AddSingleton(config);

        if (!config.GeneratedControllers) return;

        var semanticsDefiner = new SemanticsDefiner();

        services.AddControllers(o =>
        {
            o.Conventions.Add(new ApplicationModelConvention(config.EntitySettingsCollection, semanticsDefiner));

        })
        .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider(config.EntitySettingsCollection)))
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddSingleton(semanticsDefiner); // ?
    }

    private static void AddEndpoints(AutoApiRestConfigurationOptions config)
    {
        var endpointActions = typeof(IAutoApiRestController<,,>)
            .GetMethods()
            .Select(x => new ActionEndpointSettings(x.Name))
            .ToArray();

        foreach (var endpoint in config.EndpointSettingsCollection)
        {
            endpoint.Value.ActionSettingsCollection = endpointActions;
        }
    }

    public static void AddRestApi(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();

        var config = provider.GetRequiredService<AutoApiRestConfigurationOptions>();

        var entitySettings = new EntitySettingsContainer(config.EntitySettingsCollection!);

        foreach (var handlerType in new HandlerTypesContainer())
        {
            services.AddScoped(handlerType[0], handlerType[1]);
        }

        var repositoryShells = new Dictionary<string, RepositoryShell>();

        var parameterShells = new Dictionary<string, IReadOnlyDictionary<string, QueryParameterShell>>();

        foreach (var setting in entitySettings)
        {
            parameterShells.Add(setting.QueryRequestType.Name, CreateEntityParameterShells(setting));

            //  services.AddTransient(typeof(IValidator<>).MakeGenericType(setting.CommandRequestType), setting.ValidatorType);

            services.AddEntityHandlers(setting);

            var queryManager = CreateQueryManager(setting);

            var repositoryConstructor = CreateRepositoryConstructor(setting, config.ContextTypes![0], config.ExtendedRepositoryType!);

            repositoryShells.Add(setting.EntityType.Name, new RepositoryShell(repositoryConstructor, queryManager));
        }

        services.AddSingleton(new RequestProviderItems
        {
            Parameters = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>>(parameterShells),
            PaginationSettings = config.PaginationSettings ?? new PaginationSettings(),
        });

        services.AddHttpContextAccessor();

        services.AddPipelineBehaviors();

        services.AddTransient<ServiceFactory>(p => p.GetService);

        services.AddTransient<IMediator, MediatorImpl>();

        var mapper = new Mapping(entitySettings).ConfigureMapper();
        services.TryAddScoped(x => mapper);

        services.AddRepositoryWrapper(config.ContextTypes![0], mapper, repositoryShells);

        services.AddUriService();

        services.AddSwagger();
    }

    public static void AddRapierExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }

    private static ReadOnlyDictionary<string, QueryParameterShell> CreateEntityParameterShells(EntitySettings setting)
    {
        var parameterDict = new Dictionary<string, QueryParameterShell>();

        foreach (var parameter in setting.ParameterConfigurations)
        {
            parameterDict.Add(
                parameter.PropertyName,
                new QueryParameterShell(ExpressionUtility.CreateConstructor(parameter.ParameterType, typeof(string), typeof(string[])),
                parameter.NavigationNodes));
        }

        return new(parameterDict);
    }

    private static void AddEntityHandlers(this IServiceCollection services, EntitySettings setting)
    {
        var entityTypes = EntityTypes.Build(setting);
        var properties = entityTypes
            .GetType()
            .GetProperties()
            .Where(t => t.PropertyType.IsArray);

        foreach (var property in properties)
        {
            if (property.GetValue(entityTypes) is Type[] handler)
            {
                services.AddScoped(handler[0], handler[1]);
            }
        }
    }

    private static object CreateQueryManager(EntitySettings setting)
    {
        var queryConfigConstructor = ExpressionUtility.CreateConstructor(
            typeof(QueryConfiguration<>).MakeGenericType(setting.EntityType));
        //typeof(ICollection<string>));

        var queryManagerConstructor = ExpressionUtility.CreateConstructor(
            typeof(QueryManager<>).MakeGenericType(setting.EntityType),
            typeof(IQueryConfiguration));
            //typeof(ConfigurationDefinition.Entity[]));

        var queryConfig = queryConfigConstructor(setting.ExplicitExpandedMembers?.ToList()!);

        var queryManager = queryManagerConstructor(queryConfig); // , setting.ForeignEntityDefinitions);

        return queryManager;
    }

    private static ExpressionUtility.ConstructorDelegate CreateRepositoryConstructor(EntitySettings setting, Type contextType, Type extendedRepositoryType)
    {
        var repositoryConstructor = ExpressionUtility.CreateConstructor(
            (extendedRepositoryType ?? typeof(Repository<,,>)).MakeGenericType(setting.EntityType, setting.ResponseType, contextType),
            contextType,
            typeof(IMapper),
            typeof(QueryManager<>).MakeGenericType(setting.EntityType));

        return repositoryConstructor;
    }

    //public class PipelineBehaviorOptions
    //{
    //    public int SequenceOrder { get; init; }
    //}

    private static void AddPipelineBehaviors(this IServiceCollection services)
    {
        //  services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        //    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,,>));
        services.AddTransient(typeof(IPipelineBehaviour<,>), typeof(ProvideCommandBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehaviour<,>), typeof(ProvideQueryBehaviour<,>));
    }

    private static void AddRepositoryWrapper(this IServiceCollection services, Type dbContextType, IMapper mapper, Dictionary<string, RepositoryShell> repositories)
    {
        services.AddScoped(x =>
        {
            var context = x.GetRequiredService(dbContextType);

            var wrapperCtor = typeof(RepositoryWrapper<>)
            .MakeGenericType(dbContextType)
            .GetConstructor(new[]
            {
                dbContextType,
                typeof(IMapper),
                typeof(IReadOnlyDictionary<string,RepositoryShell>)
            });

            return (IRepositoryWrapper)wrapperCtor!.Invoke(new[]
            {
                context,
                mapper,
                new ReadOnlyDictionary<string, RepositoryShell>(repositories)
            });
        });
        //  services.Decorate<IRepositoryWrapper, CachedRepositoryWrapper>();
    }

    private static void AddUriService(this IServiceCollection services)
    {
        services.AddScoped<IUriService>(provider =>
        {
            var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext!.Request;
            var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
            return new UriService(absoluteUri);
        });
    }
}
