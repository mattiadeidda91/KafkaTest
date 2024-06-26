using Consumer.Configuration;
using Consumer.HostedService;
using Kafka.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Kafka
builder.Services.AddEventBusService(builder.Configuration);
builder.Services.ConfigureEventHandlerMsgBus();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddControllers();

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
