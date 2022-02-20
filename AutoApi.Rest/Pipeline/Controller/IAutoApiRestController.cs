using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoApi.Rest.Pipeline.Controller;

public interface IAutoApiRestController<TResponse, in TQuery, in TCommand>
//where TQuery : GetRequest
//where TCommand : IModifyRequest
{
    Task<ActionResult<PaginateableResponse<TResponse>>> Get([FromQuery] TQuery request);
    Task<ActionResult<TResponse>> Create([FromBody] TCommand request);
    Task<ActionResult<TResponse>> GetById(Guid id);
    Task<ActionResult<TResponse>> Update(Guid id, [FromBody] TCommand request);
    Task<IActionResult> Delete(Guid id);
}
