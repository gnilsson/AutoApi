using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.RequestManagement;
using AutoMapper;

namespace AutoApi.Rest.Pipeline.Handlers;

public class CreateHandler<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : ICommand, IRequest<TResponse>
{
    internal readonly IRepository<TEntity, TResponse> _repository;
    internal readonly IRepositoryWrapper _repositoryWrapper;
    internal readonly IMapper _mapper;

    private readonly Func<TRequest, TEntity> _create;

    public CreateHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IModifier<TEntity, TRequest> modifier)
    {
        _repositoryWrapper = repositoryWrapper;
        _repository = repositoryWrapper.Get<TEntity, TResponse>();
        _mapper = mapper;
        _create = modifier.Create;
    }

    public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var entity = _create(request);

        await _repository.CreateAsync(entity, cancellationToken);

        await _repositoryWrapper.SaveAsync();

        return _mapper.Map<TResponse>(entity);
    }
}
