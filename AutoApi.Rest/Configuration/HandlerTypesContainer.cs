using AutoApi.Domain;
using AutoApi.Mediator;
using AutoApi.Rest.Pipeline.Handlers;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.Shared.Requests;

namespace AutoApi.Rest.Configuration;

//todo make interfaces nested in class
public interface IRequestHandlerCustomCreate<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : ICommand, IRequest<TResponse>
{ }

public interface IRequestHandlerCustomGet<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : IRequest<TResponse>
{ }

public interface IRequestHandlerCustomGetById<TEntity, TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : GetByIdQuery<TResponse>
{ }

public interface IPipelineBehaviorCustom<TEntity, TRequest, TResponse> :
    IPipelineBehaviour<TRequest, TResponse>
    where TEntity : IEntity
    where TRequest : ICommand, IRequest<TResponse>
{ }

public class HandlerTypesContainer : List<Type[]>
{
    public HandlerTypesContainer()
    {
        AddRange(new Type[][]
        {
            new [] { typeof(IRequestHandlerCustomGet<,,>), typeof(GetHandler<,,>) },
            new [] { typeof(IRequestHandlerCustomCreate<,,>), typeof(CreateHandler<,,>) },
            new [] { typeof(IRequestHandlerCustomGetById<,,>), typeof(GetByIdHandler<,,>) },
            new [] { typeof(IRequestHandlerCustomCreate<,,>), typeof(UpdateHandler<,,>) },
            new [] { typeof(IRequestHandler<,>), typeof(DeleteHandler<,>) },
            new [] { typeof(IModifier<,>), typeof(Modifier<,>) },
        });
    }
}
