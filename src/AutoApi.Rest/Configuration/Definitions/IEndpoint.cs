using AutoApi.Rest.Configuration.Wire;

namespace AutoApi.Rest.Configuration.Service;

public interface IEndpoint
{
    public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; }
}
