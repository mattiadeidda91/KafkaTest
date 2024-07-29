using Common.Extensions;
using Common.Interfaces;
using Common.Options;
using Common.Service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Options
_ = builder.Services.BuildOptions<JwtTokenOptions>(builder.Configuration);

//Services
builder.Services.AddScoped<IJwtBearerToken, JwtBearerToken>();

builder.Services.AddControllers();

//Add Serilog
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
