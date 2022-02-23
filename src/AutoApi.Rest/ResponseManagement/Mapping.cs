using AutoApi.Domain;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Shared.Responses;
using AutoApi.Toolkit;
using AutoApi.Utility;
using AutoMapper;
using GN.Toolkit;

namespace AutoApi.Rest.ResponseManagement;

//public class IdentifierTypeConverter : ITypeConverter<Guid, Identifier>
//{
//    public Identifier Convert(Guid source, Identifier destination, ResolutionContext context)
//    {
//        return new Identifier(source);
//    }
//}

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
            //.ForMember(
            //    x => x.Id,
            //    o => o.MapFrom(x => new Identifier(x.Id)));

            foreach (var setting in _settings)
            {
                //if (setting.EntityType.GetProperties().Select(x => x.Name).Contains("BlogId"))
                //{
                //    map.ForMember("BlogId", o => o.MapFrom(x => new Identifier(x.Id)));
                //}

            //    cfg.CreateMap<Guid, Identifier>().ConvertUsing<IdentifierTypeConverter>();

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
