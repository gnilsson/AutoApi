using AutoApi.Rest.Configuration.Service;

namespace AutoApi.Rest.Configuration.Settings;

public class ControllerEndpointSettings : EndpointBase, IEndpoint
{
    public string Route { get; init; } = default!;
    public ActionEndpointSettings[] ActionSettingsCollection { get; set; } = default!;
    public bool AutoExpandMembers { get; set; } = true;
    public string[]? ExplicitExpandedMembers { get; set; }
}
