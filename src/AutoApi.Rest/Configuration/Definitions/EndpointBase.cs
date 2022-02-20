using AutoApi.Rest.Configuration.Wire;

namespace AutoApi.Rest.Configuration.Service;

public abstract class EndpointBase
{
    public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; } = new();
}
