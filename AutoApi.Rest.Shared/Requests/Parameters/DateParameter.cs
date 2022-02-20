namespace AutoApi.Rest.Shared.Requests.Parameters;

public class DateParameter : IParameter
{
    public object Value { get; private set; }
    public string[] NestingNavigationProperties { get; internal set; }
    public string[] LocalNavigationProperties { get; internal set; }
    public string Method { get; private set; }
    public string[] NestingCollectionNavigationProperties { get; }

    public virtual void Set(string value)
    {
        Value = DateTime.Parse(value);
        Method = "CallDateTimeCompare"; //QueryMethod.CallDateTimeCompare;
    }

    public DateParameter(string value, string[] navigationNodes)
    {
        this.Set(value);
        LocalNavigationProperties = navigationNodes;
    }
}
