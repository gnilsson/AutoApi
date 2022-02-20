using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AutoApi.Rest.Configuration;

public static class SwaggerConfigurationExtensions
{
    private static readonly Action _increment;
    private static readonly Func<int> _getCount;

    static SwaggerConfigurationExtensions()
    {
        var count = 0;
        _increment = () => count++;
        _getCount = () => count;
    }
    public static void AddSwagger(this IServiceCollection services) //Action<SwaggerGenOptions> swaggerConfiguration)
    {
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo { Title = "Rapier.Server API", Version = "v1" });

            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the bearer scheme",
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id ="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            x.DocInclusionPredicate((x, y) =>
            {

                return true;
            });
            x.ResolveConflictingActions(x =>
            {
                foreach (var de in x)
                {
                    return de;
                }
                return x.First();
            });
            x.CustomOperationIds(e =>
            {
                var id = e.ActionDescriptor.RouteValues["action"];
                if (id == "Get")
                {
                    var count = _getCount();
                    id = $"{id}{count}";
                    _increment();
                }
                return id;
            });
            // getActionNameFunc
            //      x.OperationFilter<RapierOperationFilter>();
            // x.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            //// x.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");

        });

        //services.AddSwaggerGen(opt =>
        //{
        //    opt = swaggerConfiguration.Invoke();
        //    opt.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
        //});
    }
}
