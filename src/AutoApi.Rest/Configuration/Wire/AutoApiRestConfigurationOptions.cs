using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.EntityFramework.RequestManagement;

namespace AutoApi.Rest.Configuration.Wire;

public class AutoApiRestConfigurationOptions
{
    public Type[] ContextTypes { get; set; } = default!;
    public EntitySettings[] EntitySettingsCollection { get; set; } = default!;
    public Dictionary<Type, ControllerEndpointSettings> EndpointSettingsCollection { get; set; } = default!;
    public bool GeneratedControllers { get; set; } = true;
    public bool InterfaceDiscovery { get; set; } = true;
    public bool RoutesByAttribute { get; set; } = false;
    public Type? ExtendedRepositoryType { get; set; }
    public PaginationSettings? PaginationSettings { get; set; }
    //public Action<SwaggerGenOptions> SwaggerConfiguration { get; set; }

}
