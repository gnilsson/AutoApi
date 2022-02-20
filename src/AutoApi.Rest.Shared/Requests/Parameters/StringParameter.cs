namespace AutoApi.Rest.Shared.Requests.Parameters;

public class StringParameter : IParameter
{
    public string[]? NestingNavigationProperties { get; protected set; }
    public string[]? LocalNavigationProperties { get; protected set; }
    public object? Value { get; private set; }
    public string? Method { get; private set; }
    public string[]? NestingCollectionNavigationProperties { get; }

    public virtual void Set(string value)
    {
        Value = value;
        Method = "CallStringContains"; //QueryMethod.CallStringContains;
    }

    public StringParameter(string value, string[] navigationNodes)
    {
        this.Set(value);
        LocalNavigationProperties = navigationNodes;
    }
}
