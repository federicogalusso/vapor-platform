using Interfaces.Repositories;
using Interfaces.Services;
using Interfaces.Statistics.Services;
using Statistics.Data;
using WebApiRabbitMQ.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<EventProcessor>();
builder.Services.AddSingleton<IStatisticsService, StatisticsService>();
builder.Services.AddSingleton<ReportGeneratorService>();
builder.Services.AddSingleton<EventProcessor>();
builder.Services.AddSingleton<GameDataAccess>();
builder.Services.AddSingleton<UserDataAccess>();
builder.Services.AddSingleton<EventDataAccess>();





var app = builder.Build();

var eventProcessor = app.Services.GetRequiredService<EventProcessor>();

await Task.Run(() => eventProcessor.StartListening());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Urls.Add("http://0.0.0.0:5062");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();