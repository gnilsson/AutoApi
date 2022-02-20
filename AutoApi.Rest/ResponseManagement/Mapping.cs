using AutoApi.Domain;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Shared.Responses;
using AutoApi.Toolkit;
using AutoApi.Utility;
using AutoMapper;

namespace AutoApi.Rest.ResponseManagement;

public class Mapping
{
    private readonly EntitySettingsContainer _settings;

    public Mapping(EntitySettingsContainer settings) => _settings = settings;

    public IMapper ConfigureMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod!.IsPublic || p.GetMethod.IsAssembly;

            cfg.CreateMap<IEntity, EntityResponse>()
            .IncludeAllDerived()
            .ForMember(
                x => x.CreatedDate,
                o => o.MapFrom(x => x.CreatedDate.ToLongDateTimeString()))
            .ForMember(
                x => x.UpdatedDate,
                o => o.MapFrom(x => x.UpdatedDate.ToLongDateTimeString()));

            foreach (var setting in _settings)
            {
                cfg.CreateMap(setting.EntityType, setting.ResponseType).ForMembersExplicitExpansion(setting);

                if (setting.ResponseType.ParentHasInterface(typeof(ISimplified), out var parent))
                {
                    cfg.CreateMap(setting.EntityType, parent).As(setting.ResponseType);
                }
            }
        });

        return config.CreateMapper();
    }
}
