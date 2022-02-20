using AutoApi.Defined.Descriptive;
using AutoApi.Descriptive;
using AutoApi.Mediator;
using AutoApi.QueryRequestDefinition;
using AutoApi.QueryRequestDefinition.Parameters;
using AutoApi.Rest.Configuration;
using AutoApi.Rest.EntityFramework.RequestManagement;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.Shared.Requests.Parameters;
using Microsoft.AspNetCore.Http;

namespace AutoApi.Rest.Pipeline.PipelineBehaviours;

internal sealed class ProvideQueryBehaviour<TRequest, TResponse> :
    IPipelineBehaviour<TRequest, TResponse>
    where TRequest : QueryRequest
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>> _parameters;
    private readonly PaginationSettings _paginationSettings;
    private readonly HttpContext _httpContext;
    private readonly SemanticsDefiner.Query<TResponse> _querySemantics;

    public ProvideQueryBehaviour(RequestProviderItems providerItems, IHttpContextAccessor accessor, SemanticsDefiner semanticsDefiner)
    {
        _parameters = providerItems.Parameters;
        _paginationSettings = providerItems.PaginationSettings;
        _httpContext = accessor.HttpContext!;
        _querySemantics = semanticsDefiner.GetQuery<TResponse>();
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        request.Parameters = _parameters
            .FirstOrDefault(x => x.Key == request.Query.GetType().Name).Value
            .Where(x => _httpContext.Request.Query.ContainsKey(x.Key))
            .Select(x => x.Value.Constructor(_httpContext.Request.Query[x.Key].ToString(), x.Value.NavigationArgs) as IParameter)
            .ToList()!;

        request.PaginationQuery = new PaginationQuery(request.Query, _paginationSettings);
        request.RequestRoute = _httpContext.Request.Path;

        if (!string.IsNullOrWhiteSpace(request.Query.OrderBy))
        {
            AddOrderByParameter(request);
        }

        if (!string.IsNullOrWhiteSpace(request.Query.Expand))
        {
            AddExpandMembers(request);
        }

        return await next();
    }

    private void AddOrderByParameter(QueryRequest request)
    {
        if (!request.Query.OrderBy!.Contains(':'))
        {
            AddError(request, ErrorMessage.Query.OrderParameterSeperator);
            return;
        }

        var orderQuery = request.Query.OrderBy.Split(":");
        if (!orderQuery[0].Contains(OrderByParameterDescription.Ascending, StringComparison.OrdinalIgnoreCase) &&
            !orderQuery[0].Contains(OrderByParameterDescription.Descending, StringComparison.OrdinalIgnoreCase))
        {
            AddError(request, ErrorMessage.Query.OrderParameterDescriptor);
            return;
        }

        if (!_querySemantics.DefaultFields.Contains(orderQuery[0]))
        {
            AddError(request, ErrorMessage.Query.OrderParameterField);
            return;
        }

        request.OrderByParameter = new OrderByParameter(orderQuery);
    }

    private void AddExpandMembers(QueryRequest request)
    {
        var expandMembers = new List<string>();
        foreach (var member in request.Query.Expand!.Split('.'))
        {
            if (_querySemantics.RelationalFields.Contains(member))
            {
                expandMembers.Add(member);
                continue;
            }

            AddError(request, ErrorMessage.Query.ExpandParameterField);
        }

        request.ExpandMembers = expandMembers.ToArray();
    }

    private static void AddError(QueryRequest request, string errorMessage)
    {
        request.Errors ??= new List<string>();
        request.Errors.Add(errorMessage);
    }
}
