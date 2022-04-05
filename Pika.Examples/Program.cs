using Pika.Examples;
using PikaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConnector<ExampleUrlProvider>();
builder.Services.AddPublisher<IExamplePublisher, ExamplePublisher>();
builder.Services.AddConsumer<ExampleConsumer>();
builder.Services.AddNotifier<ExampleNotifier>();

var app = builder.Build();

app.Run();
