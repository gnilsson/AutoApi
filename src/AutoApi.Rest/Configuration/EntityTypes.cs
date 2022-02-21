using AutoApi.Mediator;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Pipeline.Handlers;
using AutoApi.Rest.RequestManagement;
using AutoApi.Rest.Shared.Requests;
using AutoApi.Rest.Shared.Responses;

namespace AutoApi.Rest.Configuration;

public class EntityTypes
{
    public Type QueryConfiguration { get; init; }
    public Type[] GetHandler { get; init; }
    public Type[] CreateHandler { get; init; }
    public Type[] Modifier { get; init; }
    public Type[] CreateValidator { get; init; }
    public Type[] UpdateValidator { get; init; }
    public Type[] UpdateHandler { get; init; }
    public Type[] GetByIdHandler { get; init; }
    public Type[] DeleteHandler { get; init; }

    public static EntityTypes Build(EntitySettings setting)
    {
        var getQuery = typeof(GetQuery<,>).MakeGenericType(setting.QueryRequestType, setting.ResponseType);
        var createCommand = typeof(CreateCommand<,>).MakeGenericType(setting.CommandRequestType, setting.ResponseType);
        var updateCommand = typeof(UpdateCommand<,>).MakeGenericType(setting.CommandRequestType, setting.ResponseType);
        var getByIdQuery = typeof(GetByIdQuery<>).MakeGenericType(setting.ResponseType);
        var deleteCommand = typeof(DeleteCommand);
        var commandReciever = typeof(CommandRequest<>).MakeGenericType(setting.CommandRequestType);

        var entityTypes = new EntityTypes
        {
            QueryConfiguration = setting.QueryConfigurationType,

            GetHandler = new[]
            {
                typeof(IRequestHandler<,>)
                    .MakeGenericType(getQuery, typeof(PaginateableResponse<>)
                    .MakeGenericType(setting.ResponseType)),

                typeof(GetHandler<,,>)
                    .MakeGenericType(setting.EntityType, getQuery, setting.ResponseType)
            },

            CreateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                    .MakeGenericType(createCommand, setting.ResponseType),

                typeof(CreateHandler<,,>)
                    .MakeGenericType(setting.EntityType, createCommand, setting.ResponseType)
            },

            UpdateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                    .MakeGenericType(updateCommand, setting.ResponseType),

                typeof(UpdateHandler<,,>)
                    .MakeGenericType(setting.EntityType, updateCommand, setting.ResponseType)
            },

            GetByIdHandler = new[]
            {
                typeof(IRequestHandler<,>)
                    .MakeGenericType(getByIdQuery, setting.ResponseType),

                typeof(GetByIdHandler<,,>)
                    .MakeGenericType(setting.EntityType, getByIdQuery, setting.ResponseType)
            },

            DeleteHandler = new[]
            {
                typeof(IRequestHandler<,>)
                    .MakeGenericType(deleteCommand, typeof(DeleteResponse)),

                typeof(DeleteHandler<,>)
                    .MakeGenericType(setting.EntityType, deleteCommand)
            },

            Modifier = new[]
            {
                typeof(IModifier<,>)
                    .MakeGenericType(setting.EntityType, commandReciever),

                typeof(Modifier<,>)
                    .MakeGenericType(setting.EntityType, commandReciever)
            }

            //            CreateValidator = new[]
            //            {
            //                    typeof(IPipelineBehaviour<,>)
            //                    .MakeGenericType(createCommand, setting.ResponseType),

            //                    typeof(ValidationBehaviour<,,>)
            //                    .MakeGenericType(createCommand, setting.ResponseType, setting.CommandRequestType)
            //},

            //            UpdateValidator = new[]
            //            {
            //                    typeof(IPipelineBehavior<,>)
            //                    .MakeGenericType(updateCommand, setting.ResponseType),

            //                    typeof(ValidationBehaviour<,,>)
            //                    .MakeGenericType(updateCommand, setting.ResponseType, setting.CommandRequestType)
            //                }
        };

        return entityTypes;
    }
}
