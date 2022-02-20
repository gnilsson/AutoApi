using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoApi.Rest.ResponseManagement;

public static class ResponseExtensions
{
    public static async Task<ActionResult<TResponse>> ToResult<TResponse>(
        this Task<TResponse> resultTask,
        Func<object, OkObjectResult> okObject)
    {
        var result = await resultTask;
        return result is null ? new NotFoundResult() : okObject(result);
    }

    public static async Task<ActionResult<PaginateableResponse<TResponse>>> ToResult<TResponse>(
        this Task<PaginateableResponse<TResponse>> resultTask,
        Func<object, OkObjectResult> okObject)
    {
        var result = await resultTask;
        return result is null ? new NotFoundResult() : okObject(result);
    }

    public static async Task<ActionResult<TResponse>> ToResult<TResponse>(
        this Task<TResponse> resultTask,
        Func<string, object, object, CreatedAtActionResult> createdAt,
        string action)
        where TResponse : EntityResponse
    {
        var result = await resultTask;
        return result is null ? new NotFoundResult() : createdAt(action, new { id = result.Id }, result);
    }
}
