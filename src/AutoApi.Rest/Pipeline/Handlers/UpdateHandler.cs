using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.RequestManagement;
using AutoMapper;

namespace AutoApi.Rest.Pipeline.Handlers;

public class UpdateHandler<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : ICommand, IRequest<TResponse>
{
    internal readonly IRepository<TEntity, TResponse> _repository;
    internal readonly IRepositoryWrapper _repositoryWrapper;
    internal readonly IMapper _mapper;

    private readonly Action<TEntity, TRequest> _update;

    public UpdateHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IModifier<TEntity, TRequest> modifier)
    {
        _repositoryWrapper = repositoryWrapper;
        _repository = repositoryWrapper.Get<TEntity, TResponse>();
        _mapper = mapper;
        _update = modifier.Update;
    }

    public virtual async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken)
    {
        var entity = request.IncludeNavigation == null ?
           await _repository.FindAsync(request.Id, cancellationToken) :
           await _repository.GetOneAsync(x => x.Id == request.Id, request.IncludeNavigation, cancellationToken);

        if (entity is null) return default!;

        _update(entity, request);

        await _repositoryWrapper.SaveAsync();

        return _mapper.Map<TResponse>(entity);
    }
}
