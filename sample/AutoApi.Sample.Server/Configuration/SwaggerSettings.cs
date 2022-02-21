using GN.Toolkit;

namespace AutoApi.Sample.Server.Configuration;

internal class SwaggerSettings : SettingsBase<SwaggerSettings>
{
    public string? JsonRoute { get; init; }
    public string? Description { get; init; }
    public string? UIEndpoint { get; init; }
}
