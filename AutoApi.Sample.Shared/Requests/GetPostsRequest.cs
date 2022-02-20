using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Requests.Parameters;

namespace AutoApi.Sample.Shared.Requests;

public class GetPostsRequest : GetRequest
{
    [QueryParameter(typeof(StringParameter))]
    public string Title { get; set; }
    public string Content { get; set; }
    [QueryParameter(typeof(ForeignIdParameter))]
    public Guid BlogId { get; set; }
}
