using Consumer.Configuration;
using Consumer.HostedService;
using Kafka.Configurations;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Common.Configurations.Swagger;
using System.Text.Json.Serialization;
using System.Text.Json;
using Common.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Kafka
builder.Services.AddEventBusService(builder.Configuration);
builder.Services.ConfigureEventHandlerMsgBus();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.BuildControllerConfigurations();

//Add Serilog
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetEntryAssembly()?.GetName().Name, Version = "v1" });
    options.OperationFilter<OperationFilter>();
});

//Add Api versioning using namespace convention
builder.Services.UseApiVersioningNamespaceConvention();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Serilog log all requests
app.UseSerilogRequestLogging(options =>
{
    options.IncludeQueryInRequestPath = true;
});

app.UseHttpsRedirection();

//Handle Errors
app.UseErrorHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
