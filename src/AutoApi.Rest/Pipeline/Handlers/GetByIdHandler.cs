using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.Shared.Requests;
using AutoMapper;

namespace AutoApi.Rest.Pipeline.Handlers;

public class GetByIdHandler<TEntity, TRequest, TResponse> :
        IRequestHandler<TRequest, TResponse>
        where TEntity : IEntity
        where TRequest : GetByIdQuery<TResponse>
{
    internal readonly IRepository<TEntity, TResponse> _repository;
    internal readonly IMapper _mapper;

    public GetByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repository = repositoryWrapper.Get<TEntity, TResponse>();
        _mapper = mapper;
    }

    public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var entityResponse = await _repository.GetOneAsync(x => x.Id == (Guid)request.Id , cancellationToken);

        return entityResponse ?? default!;
    }
}
