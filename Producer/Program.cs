using Common.Configurations.Swagger;
using Common.Extensions;
using Common.Options;
using Common.Utils;
using Kafka.Configurations;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Get Api versions
var apiVersions = ApiVersionHelper.GetApiVersions(Assembly.GetExecutingAssembly());

//Configure Options
var jwtOptions = builder.Services.BuildOptions<JwtTokenOptions>(builder.Configuration);

builder.Services.AddEventBusService(builder.Configuration);

//Configure Authentication
builder.Services.AuthenticationBuild(builder.Environment.IsProduction(), jwtOptions);

//Configure Authorization
builder.Services.AuthorizationBuild();

//Configure Controllers
builder.Services.BuildControllerConfigurations();

//Configure Serilog
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

//Configure Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Configure Swagger
builder.Services.SwaggerBuild(apiVersions);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure Api versioning using namespace convention
builder.Services.UseApiVersioningNamespaceConvention();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (var version in apiVersions)
        {
            c.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
        }
    });
}

//Serilog logs all requests
app.UseSerilogRequestLogging(options =>
{
    options.IncludeQueryInRequestPath = true;
});

app.UseHttpsRedirection();

//Handle Errors
app.UseErrorHandlingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
