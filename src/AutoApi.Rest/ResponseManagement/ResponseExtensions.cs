using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoApi.Rest.ResponseManagement;

public static class ResponseExtensions
{
    public static async Task<IActionResult> ToResultAsync<TResponse>(
        this Task<TResponse> resultTask,
        Func<object, OkObjectResult> okObject,
        Func<IActionResult> notFound)
    {
        var result = await resultTask;
        return result is null ? notFound() : okObject(result);
    }

    public static async Task<IActionResult> ToResultAsync<TResponse>(
        this Task<PaginateableResponse<TResponse>> resultTask,
        Func<object, OkObjectResult> okObject,
        Func<IActionResult> notFound)
    {
        var result = await resultTask;
        return result is null ? notFound() : okObject(result);
    }

    public static async Task<IActionResult> ToResultAsync<TResponse>(
        this Task<TResponse> resultTask,
        string action,
        Func<string, object, object, CreatedAtActionResult> createdAt,
        Func<IActionResult> notFound) where TResponse : EntityResponse
    {
        var result = await resultTask;
        return result is null ? notFound() : createdAt(action, new { id = result.Id }, result);
    }
}
