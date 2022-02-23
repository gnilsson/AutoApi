using AutoApi.Mediator;
using AutoApi.Rest.EntityFramework.RequestManagement;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Rest.RequestManagement;

public class GetQuery<TQuery, TResponse> :
    QueryRequest,
    IRequest<PaginateableResponse<TResponse>>
    where TQuery : IGetRequest
{
    public GetQuery(TQuery request) : base(request) { }
}
