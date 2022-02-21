using System.Reflection;
using AutoApi.Descriptive;
using AutoApi.Rest.Configuration.Builders;
using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Configuration.Wire;
using AutoApi.Rest.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoApi.Rest.Configuration;

public class ApplicationModelConvention : IApplicationModelConvention
{
    private readonly IList<EntitySettings> _settingsCollection;
    private readonly SemanticsDefiner _semanticsDefiner;

    public ApplicationModelConvention(IList<EntitySettings> settings, SemanticsDefiner semanticsDefiner)
    {
        _settingsCollection = settings;
        _semanticsDefiner = semanticsDefiner;
    }

    public void Apply(ApplicationModel application)
    {
        var controllerCount = application.Controllers.Count;

        var actionDescriptions = new List<ConfigurationDefinition.Action>();

        foreach (var settings in _settingsCollection)
        {
            var index = _settingsCollection.IndexOf(settings);

            var dumpController = application.Controllers[index];

            var controllerModel = BuildControllerModel(application, settings, dumpController);

            foreach (var dumpAction in dumpController.Actions)
            {
                var actionModel = BuildActionModel(settings, controllerModel, dumpAction);

                if (actionModel is null) continue;

                actionDescriptions.Add(new(settings.ResponseType, actionModel.ActionName, controllerModel.ControllerName, actionModel.ActionMethod.Name, 0));

                controllerModel.Actions.Add(actionModel);
            }

            application.Controllers.Add(controllerModel);
        }

        _semanticsDefiner.Configure(
            actionDescriptions,
            new Dictionary<Type, IEnumerable<ConfigurationDefinition.Field>>(_settingsCollection.Select(x => x.FieldDescriptions)));

        for (int i = 0; i < controllerCount; i++)
        {
            application.Controllers.RemoveAt(0);
        }
    }

    private static ControllerModel BuildControllerModel(ApplicationModel application, EntitySettings settings, ControllerModel dumpController)
    {
        var controllerType = DynamicAssemblyBuilder.ReflectionContainers[settings.EntityType.Name].CreatedType!.GetTypeInfo();

        var attributes = new List<object>(dumpController.Attributes)
        {
            new ApiConventionTypeAttribute(typeof(DefaultApiConventions)),
        };

        var controllerModel = new ControllerModel(controllerType, attributes)
        {
            Application = application,
            ControllerName = $"{settings.EntityType.Name}Controller",
        };

        controllerModel.Filters.Add(new ProducesAttribute("application/json"));

        foreach (var cp in dumpController.ControllerProperties)
        {
            controllerModel.ControllerProperties.Add(cp);
        }

        return controllerModel;
    }

    private static ActionModel BuildActionModel(EntitySettings settings, ControllerModel controllerModel, ActionModel dumpAction)
    {
        var actionName = dumpAction.ActionName switch
        {
            DefaultAction.Get => $"{dumpAction.ActionName}{settings.EntityType.Name}s",
            DefaultAction.GetById => CreateGetByIdActionName(dumpAction.ActionName, settings.EntityType.Name),
            DefaultAction.Create or DefaultAction.Update or DefaultAction.Delete => $"{dumpAction.ActionName}{settings.EntityType.Name}",
            _ => dumpAction.ActionName,
        };

        var attributes = new List<object>(dumpAction.Attributes)
        {
            new ActionNameAttribute(actionName)
        };

        var actionModel = new ActionModel(dumpAction.ActionMethod, attributes)
        {
            Controller = controllerModel,
            ActionName = actionName
        };

        foreach (var param in dumpAction.Parameters)
        {
            actionModel.Parameters.Add(param);
        }

        foreach (var selectorModel in dumpAction.Selectors)
        {
            var template = string.Empty;

            if (dumpAction.ActionName is DefaultAction.GetById or DefaultAction.Update or DefaultAction.Delete)
            {
                template = $"{settings.ControllerRoute}/{{id}}";
            }
            else if (dumpAction.ActionName.Contains("Nested"))
            {
                var foreign = settings.ForeignEntityDefinitions.FirstOrDefault(x => dumpAction.ActionName.Contains(x.EntityType.Name));

                template = $"{settings.ControllerRoute}/{{id}}/{foreign!.EntityType.Name.ToLower()}s";
            }
            else
            {
                template = settings.ControllerRoute;
            }

            actionModel.Selectors.Add(new SelectorModel(selectorModel)
            {
                AttributeRouteModel = new AttributeRouteModel { Template = template }
            });
        }

        foreach (var actionFilter in GetActionFilters(settings, controllerModel, actionModel))
        {
            actionModel.Filters.Add(actionFilter);
        }

        return actionModel;
    }

    private static IEnumerable<IFilterMetadata> GetActionFilters(EntitySettings setting, ControllerModel controllerModel, ActionModel actionModel)
    {
        if (setting.AuthorizeableEndpoints.TryGetValue($"{controllerModel.ControllerType.FullName}.{actionModel.ActionMethod.Name}", out var endpoint)
            && endpoint.Category is not AuthorizationCategory.None)
        {
            yield return new AuthorizeFilter(endpoint.Category == AuthorizationCategory.Custom ? endpoint.Policy : string.Empty);
        }

        yield return actionModel.ActionMethod.Name switch
        {
            DefaultAction.Get => new ProducesResponseTypeAttribute(typeof(PaginateableResponse<>).MakeGenericType(setting.ResponseType), 200),
            DefaultAction.Create => new ProducesResponseTypeAttribute(setting.ResponseType, 201),
            DefaultAction.Delete => new ProducesResponseTypeAttribute(typeof(DeleteResponse), 200),
            _ => new ProducesResponseTypeAttribute(setting.ResponseType, 200)
        };

        yield return new ProducesResponseTypeAttribute(typeof(NotFoundResult), 404);
    }

    private static string CreateGetByIdActionName(string actionName, string entityName)
    {
        var getById = actionName.Insert(3, ".").Split('.');

        return $"{getById[0]}{entityName}{getById[1]}";
    }
}
