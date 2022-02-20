namespace AutoApi.Mediator;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse> where TRequest : IRequest<TResponse>
{
    public override Task<TResponse> Handle(
        IRequest<TResponse> request,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        Task<TResponse> Handler() => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request, cancellationToken);

        return serviceFactory
            .GetInstances<IPipelineBehaviour<TRequest, TResponse>>()
            !.Reverse()
            .Aggregate(
                (RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => () => pipeline.HandleAsync((TRequest)request, next, cancellationToken))();
    }
}
