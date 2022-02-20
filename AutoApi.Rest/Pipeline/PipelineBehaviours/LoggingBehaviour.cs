using AutoApi.Mediator;
using AutoApi.Rest.Pipeline.Handlers;

namespace AutoApi.Rest.Pipeline.PipelineBehaviours;

public class LoggingBehaviour : IPipelineBehaviour<TestRequest, TestResponse>
{
    public async Task<TestResponse> HandleAsync(TestRequest request, RequestHandlerDelegate<TestResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine("hello");
        return await next();
    }
}
