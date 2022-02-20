using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.Sample.Shared.Requests;

public class GetBlogsRequest : GetRequest
{
    [QueryParameter(typeof(StringParameter))]
    public string Title { get; set; }
}
