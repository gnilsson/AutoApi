using AutoApi.Extensions.Microsoft.DependencyInjection;
using AutoApi.Mediator;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Configuration.Wire;
using AutoApi.Rest.Pipeline.Handlers;
using AutoApi.Sample.Server.Configuration;
using AutoApi.Sample.Server.Database;
using AutoApi.Sample.Server.Descriptive;
using AutoApi.Sample.Shared;
using AutoApi.Sample.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApi.Sample.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IPipelineBehaviour<TestRequest, TestResponse>, TestPipelineBehaviour>();

        services.AddAutoApiControllers(opt =>
        {
            opt.ContextTypes = new[] { typeof(AutoApiEfDbContext), typeof(IAssemblyMarker) };

            opt.EndpointSettingsCollection = new()
            {
                [typeof(Author)] = new ControllerEndpointSettings { Route = "api/author" },
                [typeof(Blog)] = new ControllerEndpointSettings { Route = "api/blog" },
                [typeof(Post)] = new ControllerEndpointSettings { Route = "api/post" },
            };
        });

        //services.AddSwaggerGen(c =>
        //{
        //    c.SwaggerDoc("v1", new() { Title = "AutoApi.Sample.Server", Version = "v1" });
        //});

        services.AddDbContext<AutoApiEfDbContext>(options => options
            .UseSqlServer(Configuration.GetConnectionString("sqlConn"))
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging());

        //services.AddCors(ConfigNames.CorsPolicy);
        //services.AddJwt(Configuration);
        //services.AddAuthorizationHandlers();

        //// services.AddSwagger();
        //services.AddSwaggerDocument();

        services.AddAutoApi();
    }



    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoApi.Sample.Server v1"));
            app.ConfigureSwagger(Configuration);
            app.UseOpenApi();
        }

        app.AddRapierExceptionMiddleware();
        //app.ConfigureSwagger(Configuration);
        //app.UseOpenApi();
        //app.UseSwaggerUi3();
        //app.UseSwaggerUI(c => )
        app.UseHttpsRedirection();
        app.UseCors(ConfigNames.CorsPolicy);
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}




//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoApi.Sample.Server v1"));
//}

//app.UseHttpsRedirection();

//app.UseRouting();
//app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapDefaultControllerRoute();
//});

//app.Run();
