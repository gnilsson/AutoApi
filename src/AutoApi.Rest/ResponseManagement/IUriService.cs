using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.ResponseManagement;

public interface IUriService
{
    public Uri GetByIdUri(string requestRoute, string id);
    public Uri GetUri(string requestRoute, IPaginateable? paginationData = null);
}
