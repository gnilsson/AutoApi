using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoApi.Rest.Pipeline.Controller;

public interface IAutoApiRestController<in TQuery, in TCommand>
//where TQuery : GetRequest
//where TCommand : IModifyRequest
{
    Task<IActionResult> Get([FromQuery] TQuery request);
    Task<IActionResult> Create([FromBody] TCommand request);
    Task<IActionResult> GetById(string id);
    Task<IActionResult> Update(Guid id, [FromBody] TCommand request);
    Task<IActionResult> Delete(Guid id);
}
