using System.Reflection;
using AutoApi.Rest.Configuration.Builders;
using AutoApi.Rest.Configuration.Settings;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace AutoApi.Rest.Configuration;
public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IEnumerable<EntitySettings> _settingsCollection;

    public GenericTypeControllerFeatureProvider(IEnumerable<EntitySettings> settings) => _settingsCollection = settings;

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var settings in _settingsCollection)
        {
            var type = DynamicAssemblyBuilder.ReflectionContainers[settings.EntityType.Name].CreatedType;

            feature.Controllers.Add(type!.GetTypeInfo());
        }
    }
}
