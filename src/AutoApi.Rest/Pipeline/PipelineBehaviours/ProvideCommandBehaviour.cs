using System.Reflection;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.Defined;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.Shared.Attributes;
using AutoApi.Rest.Shared.Enums;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Toolkit;

namespace AutoApi.Rest.Pipeline.PipelineBehaviours;

internal sealed class ProvideCommandBehaviour<TRequest, TResponse> :
       IPipelineBehaviour<TRequest, TResponse>
       where TRequest : ICommandRequest<IModifyRequest>, IRequest<TResponse>, ICommand
{
    private readonly IGeneralRepository _repository;

    public ProvideCommandBehaviour(IRepositoryWrapper repositoryWrapper)
    {
        _repository = repositoryWrapper.General;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var property in request.Command.GetType().GetProperties())
        {
            if (!TryGetRequestValue(property, request.Command, out var keyPair)) continue;

            if (!property.TryGetAttribute<IdCollectionAttribute>(out var attribute))
            {
                request.RequestPropertyValues.Add(keyPair.Key, keyPair.Value);
                continue;
            }

            var entities = await MethodFactory.GetManyAsync(attribute!.EntityType)
                .InvokeAsync(
                _repository,
                new object[] { keyPair.Value, cancellationToken });

            request.RequestPropertyValues.Add(keyPair.Key, entities!);
            request.RequestForeignEntities.Add(keyPair.Key, attribute.EntityType);
            request.IncludeNavigation = string.Join(".", keyPair.Key);
        }

        return await next();
    }

    private static bool TryGetRequestValue(
        PropertyInfo property,
        object data,
        out KeyValuePair<string, object> value)
    {
        value = default;

        var propertyValue = property.GetValue(data);

        if (propertyValue is null) return false;

        var attribute = property.GetCustomAttribute<RequestParameterAttribute>();

        if (attribute?.Mode == RequestParameterMode.Hidden) return false;

        var propertyName = GetPropertyName(property, attribute!);

        value = KeyValuePair.Create(propertyName, propertyValue);

        return !(propertyValue is int and 0 && propertyValue is string and "");
    }

    private static string GetPropertyName(PropertyInfo property,RequestParameterAttribute parameterAttribute)
    {
        var isIdCollection = property.CustomAttributes.Any(x => x.AttributeType == typeof(IdCollectionAttribute));

        return parameterAttribute == null && isIdCollection ?
            property.Name.Replace("Id", string.Empty) :
            parameterAttribute == null ?
            property.Name :
            parameterAttribute.EntityProperty!;
    }
}
