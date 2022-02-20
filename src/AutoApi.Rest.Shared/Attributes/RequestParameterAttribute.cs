using AutoApi.Rest.Shared.Enums;

namespace AutoApi.Rest.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequestParameterAttribute : Attribute
{
    public RequestParameterAttribute(string? entityProperty = null, RequestParameterMode mode = 0)
    {
        EntityProperty = entityProperty;
        Mode = mode;
    }

    public string? EntityProperty { get; }
    public RequestParameterMode Mode { get; }
}
