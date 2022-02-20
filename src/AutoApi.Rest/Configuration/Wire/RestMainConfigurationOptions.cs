namespace AutoApi.Rest.Configuration.Wire;

public enum RestEndpointsConfigurationMode
{
    Default = 1,
    Custom
}

public interface IRestConfigurationOptionsFactory
{
    internal RestMainConfigurationOptions CreateMain() => new();
}

public record RestMainConfigurationOptions
{
    internal RestMainConfigurationOptions()
    { }

}

public record RestEndpointsConfigurationOptions
{
    internal RestEndpointsConfigurationOptions()
    { }

    public RestEndpointsConfigurationMode RestEndpointsConfigurationMode { get; set; }
}

public interface IRestConfigurer
{
    public Action<RestMainConfigurationOptions> ConfigureMain();
    public Action<RestEndpointsConfigurationOptions> ConfigureEndpoints() => config => config.RestEndpointsConfigurationMode = RestEndpointsConfigurationMode.Default;
}
