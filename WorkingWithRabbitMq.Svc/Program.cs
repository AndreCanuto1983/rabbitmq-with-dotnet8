using WorkingWithRabbitMq.Infra.Services;
using WorkingWithRabbitMq.Svc.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.RabbitMqSettings();
builder.Services.DependencyInjectionSettings();
builder.Services.ServiceExtensionSettings();
builder.Services.AddHostedService<RabbitMqConsumer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomExceptionMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthcheck");
app.UseAuthorization();
app.MapControllers();
app.Run();
