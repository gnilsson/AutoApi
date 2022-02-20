using System.ComponentModel.DataAnnotations;

namespace AutoApi.Rest.Configuration.Wire;

internal class Config
{
    [Required]
    public RestEndpointsConfigurationOptions? Endpoints { get; init; }

    [Required]
    public RestMainConfigurationOptions? Main { get; init; }
}
