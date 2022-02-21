using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.Pipeline.Handlers;
using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace AutoApi.Rest.Pipeline.Controller;

public class AutoApiRestController<TResponse, TQuery, TCommand> :
    ControllerBase,
    IAutoApiRestController<TResponse, TQuery, TCommand>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public AutoApiRestController(IMediator mediator, IRepositoryWrapper repositoryWrapper)
    {
        _mediator = mediator;
        _repositoryWrapper = repositoryWrapper;
    }

    [HttpGet("/heh")]
    public async Task<IActionResult> Test()
    {
        var ah = await _mediator.SendAsync(new TestRequest());
        return Ok(ah);
    }

    public Task<ActionResult<TResponse>> Create([FromBody] TCommand request)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<PaginateableResponse<TResponse>>> Get([FromQuery] TQuery request)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<TResponse>> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<TResponse>> Update(Guid id, [FromBody] TCommand request)
    {
        throw new NotImplementedException();
    }
}
