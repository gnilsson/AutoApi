namespace AutoApi.Rest.Configuration.Definitions;

public class ConfigurationDefinition
{
    public enum FieldCategory
    {
        Default,
        Relational
    }

    public record Field(string Name, FieldCategory Category);
    public record Action(Type ResponseType, string ActionName, string Controller, string ActionMethodName, int CustomActionImplementationOrder);
    public record Parameter(Type ParameterType, string PropertyName, string[] NavigationNodes);
    public record Entity
    {
        public Type EntityType { get; init; } = null!;
        public Type ResponseType { get; init; } = null!;
        public Type QueryType { get; init; } = null!;
        public Type CommandType { get; init; } = null!;
        public Type[] ForeignCollectionTypes { get; init; } = null!;
        public Type[] ForeignCollectionOccurenceTypes { get; init; } = null!;
    }
}
