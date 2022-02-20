using AutoApi.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoApi.Rest.Pipeline.Handlers;

public class TestRequest : IRequest<TestResponse>
{

}

public class TestResponse
{
    public string? Hello { get; init; }
}

public class GetHandler2 : IRequestHandler<TestRequest, TestResponse>
{
    public GetHandler2()
    {

    }

    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse { Hello = "hello" });
    }
}
