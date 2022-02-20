namespace AutoApi.Rest.Configuration.Wire;

public enum AuthorizationCategory
{
    None,
    Default,
    Custom
}

public class AuthorizeableEndpoint
{
    public AuthorizationCategory Category { get; set; } = 0;
    public string Policy { get; set; }
}
