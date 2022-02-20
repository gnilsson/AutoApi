using AutoApi.Mediator;
using AutoApi.Rest.Pipeline.Handlers;

namespace AutoApi.Sample.Server;

public class TestPipelineBehaviour : IPipelineBehaviour<TestRequest, TestResponse>
{
    public async Task<TestResponse> HandleAsync(TestRequest request, RequestHandlerDelegate<TestResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine("hellotest");
        return await next();
    }
}
