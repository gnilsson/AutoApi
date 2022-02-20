using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.QueryRequestDefinition;
using AutoApi.Rest.EntityFramework.RequestManagement;
using AutoApi.Rest.ResponseManagement;
using AutoApi.Rest.Shared.Responses;
using AutoMapper;

namespace AutoApi.Rest.Pipeline.Handlers;

public class GetHandler<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, PaginateableResponse<TResponse>>
    where TEntity : IEntity
    where TRequest : QueryRequest, IRequest<PaginateableResponse<TResponse>>
{
    internal readonly IRepository<TEntity, TResponse> _repository;
    internal readonly IMapper _mapper;
    internal readonly IUriService _uriService;

    public GetHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IUriService uriService)
    {
        _repository = repositoryWrapper.Get<TEntity, TResponse>();
        _uriService = uriService;
        _mapper = mapper;
    }

    public virtual async Task<PaginateableResponse<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var queryData = await _repository.GetQueriedResultAsync(request, cancellationToken);

        return PaginateableResponseBuilder.Build(_uriService, request, queryData.Items, queryData.Total)!;
    }
}
