using AutoApi.Rest.Configuration.Service;

namespace AutoApi.Rest.Configuration.Settings;

public class ActionEndpointSettings : EndpointBase, IEndpoint
{
    public string ActionMethod { get; }

    public ActionEndpointSettings(string actionMethod)
    {
        ActionMethod = actionMethod;
    }
}
