namespace AutoApi.Mediator;

public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, ServiceFactory serviceFactory, CancellationToken cancellationToken);
}

