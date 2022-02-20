using AutoApi.Extensions.Microsoft.DependencyInjection;
using AutoApi.Mediator;
using AutoApi.Rest.Configuration.Settings;
using AutoApi.Rest.Pipeline.Handlers;
using AutoApi.Sample.Server;
using AutoApi.Sample.Shared.Entities;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddTransient<IPipelineBehaviour<TestRequest, TestResponse>, TestPipelineBehaviour>();

builder.Services.AddAutoApiControllers(opt =>
{
    opt.EndpointSettingsCollection = new()
    {
        [typeof(Author)] = new ControllerEndpointSettings { Route = "api/author" },
        [typeof(Blog)] = new ControllerEndpointSettings { Route = "api/blog" },
        [typeof(Post)] = new ControllerEndpointSettings { Route = "api/post" },
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AutoApi.Sample.Server", Version = "v1" });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoApi.Sample.Server v1"));
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
