using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Configuration.Wire;

namespace AutoApi.Rest.Configuration.Settings;

public class EntitySettings
{
    public Type EntityType { get; init; } = default!;
    public Type ResponseType { get; init; } = default!;
    public Type SimplifiedResponseType { get; init; } = default!;
    public string ControllerRoute { get; init; } = default!;
    public string ControllerName { get; init; } = default!;
    public Type QueryRequestType { get; init; } = default!;
    public Type CommandRequestType { get; init; } = default!;
    public Type QueryConfigurationType { get; init; } = default!;
    //    public IDictionary<string, Type> ParameterTypes { get; init; }
    public IEnumerable<ConfigurationDefinition.Parameter> ParameterConfigurations { get; init; } = default!;
    public Type ValidatorType { get; init; } = default!;
    public IDictionary<string, AuthorizeableEndpoint> AuthorizeableEndpoints { get; init; } = default!;
    public bool AutoExpandMembers { get; init; } = true;
    public string[] ExplicitExpandedMembers { get; init; } = default!;
    public string[] ResponseMembers { get; init; } = default!;
    public KeyValuePair<Type, IEnumerable<ConfigurationDefinition.Field>> FieldDescriptions { get; init; } = default!;
    public ConfigurationDefinition.Entity[] ForeignEntityDefinitions { get; init; } = default!;
}
