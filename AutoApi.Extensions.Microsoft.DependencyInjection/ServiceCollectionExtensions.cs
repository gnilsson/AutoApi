using AutoApi.Rest.Configuration.Wire;
using Microsoft.Extensions.DependencyInjection;

namespace AutoApi.Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddAutoApiControllers(this IServiceCollection services, Action<AutoApiRestConfigurationOptions> options)
    {
        services.AddRestControllers(options);
        //services.AddMediator();
    }

    public static void AddAutoApi(this IServiceCollection services)
    {
        services.AddRestApi();
    }
}
