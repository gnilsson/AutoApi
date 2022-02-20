using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Configuration.Settings;
using AutoMapper;

namespace AutoApi.Rest.ResponseManagement;

public static class MappingExtensions
{
    public static IMappingExpression ForMembersExplicitExpansion(this IMappingExpression map, EntitySettings setting)
    {
        var relational = setting.FieldDescriptions.Value.Where(x => x.Category == ConfigurationDefinition.FieldCategory.Relational);

        if (!setting.AutoExpandMembers)
        {
            foreach (var expand in relational)
            {
                map = map.ForMember(expand.Name, x => x.ExplicitExpansion());
            }
        }

        return map;
    }
}
