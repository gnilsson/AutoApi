namespace AutoApi.Sample.Server.Configuration;

public static class SwaggerConfig
{
    public static void ConfigureSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var swaggerSettings = new SwaggerSettings().Bind(configuration)!;

        app.UseSwagger(option => { option.RouteTemplate = swaggerSettings.JsonRoute; });

        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(swaggerSettings.UIEndpoint, swaggerSettings.Description);
        });
    }
}
