using AutoApi.Rest.Shared.Requests;
using AutoApi.Utility;

namespace AutoApi.Rest.ResponseManagement;

public class UriService : IUriService
{
    private const string PageNumber = "pageNumber";
    private const string PageSize = "pageSize";

    private readonly string _baseUri;

    public UriService(string baseUri) => _baseUri = baseUri;

    public Uri GetByIdUri(string requestRoute, string id)
    {
        return new Uri(_baseUri + requestRoute.Replace(requestRoute[requestRoute.IndexOf("{")..], id));
    }

    public Uri GetUri(string requestRoute, IPaginateable? paginationData)
    {
        var uri = new Uri(_baseUri + requestRoute);

        if (paginationData is null) return uri;

        return new Uri(uri.ToString()
            .AppendQueryString(PageNumber, paginationData.PageNumber.ToString())
            .AppendQueryString(PageSize, paginationData.PageSize.ToString()));
    }
}
