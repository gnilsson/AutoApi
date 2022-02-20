using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Rest.Pipeline.Handlers;

public class DeleteHandler<TEntity, TRequest> :
             IRequestHandler<TRequest, DeleteResponse>
             where TEntity : class, IEntity
             where TRequest : DeleteCommand
{
    internal readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public virtual async Task<DeleteResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repositoryWrapper.General.FindAsync<TEntity>(request.Id, cancellationToken);

        if (entity == null) return null!;

        _repositoryWrapper.General.Delete(entity);

        await _repositoryWrapper.SaveAsync();

        return new DeleteResponse { Message = "1" };  // Todo: proper response
    }
}
