using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.ResponseManagement;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Responses;
using GN.Toolkit;
using Microsoft.AspNetCore.Mvc;

//[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace AutoApi.Rest.Pipeline.Controller;

public class AutoApiRestController<TResponse, TQuery, TCommand> :
    ControllerBase,
    IAutoApiRestController<TQuery, TCommand>
    where TResponse : IEntityResponse
    where TQuery : IGetRequest
    where TCommand : ICommandRequest
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public AutoApiRestController(IMediator mediator, IRepositoryWrapper repositoryWrapper)
    {
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TCommand request)
    {
        return await _mediator
            .SendAsync(new CreateCommand<TCommand, TResponse>(request))
            .ToResultAsync(Ok, NotFound);
    }

    [HttpDelete]
    public Task<IActionResult> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TQuery request)
    {
        return await _mediator
            .SendAsync(new GetQuery<TQuery, TResponse>(request))
            .ToResultAsync(Ok, NotFound);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(string id)
    {
        return await _mediator
            .SendAsync(new GetByIdQuery<TResponse>(new Identifier(id)))
            .ToResultAsync(Ok, NotFound);
    }

    [HttpPatch]
    public Task<IActionResult> Update(Guid id, [FromBody] TCommand request)
    {
        throw new NotImplementedException();
    }
}
