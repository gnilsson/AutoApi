namespace AutoApi.Rest.Shared.Requests.Parameters;

public interface IParameter
{
    public string[]? NestingNavigationProperties { get; }
    public string[]? LocalNavigationProperties { get; }
    public string[]? NestingCollectionNavigationProperties { get; }
    public object? Value { get; }
    public string? Method { get; }
    public void Set(string value);
}
