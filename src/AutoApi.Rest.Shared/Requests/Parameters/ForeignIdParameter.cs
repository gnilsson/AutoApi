namespace AutoApi.Rest.Shared.Requests.Parameters;

public class ForeignIdParameter : IParameter
{
    public string[] NestingNavigationProperties { get; protected set; }
    public string[] LocalNavigationProperties { get; protected set; }
    public object Value { get; private set; }
    public string Method { get; private set; }
    public string[] NestingCollectionNavigationProperties { get; }

    public void Set(Guid value)
    {

    }

    public void Set(string value)
    {
        Value = Guid.Parse(value);
        Method = "Equal"; //QueryMethod.Equal;
    }

    public ForeignIdParameter(string value, string[] navigationNodes)
    {
        this.Set(value);
        LocalNavigationProperties = navigationNodes;
    }

    public ForeignIdParameter(Guid value, string[] navigationNodes)
    {
        this.Set(value);
        LocalNavigationProperties = navigationNodes;
    }
}
