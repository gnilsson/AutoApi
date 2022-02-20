using System.Collections.Concurrent;

namespace AutoApi.Mediator;

public class MediatorImpl : IMediator
{
    private readonly ServiceFactory _serviceFactory;
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlers = new();

    public MediatorImpl(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();

        var handler = (RequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd(
            requestType,
            static t => (RequestHandlerBase)(Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(t, typeof(TResponse)))
            ?? throw new InvalidOperationException($"Could not create wrapper type for {t}")));

        return handler.Handle(request, _serviceFactory, cancellationToken);
    }

}
