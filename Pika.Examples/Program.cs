using Pika.Examples;
using PikaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConnector<ExampleUrlProvider>();
builder.Services.AddPublisher<ExamplePublisher>();
builder.Services.AddConsumer<ExampleConsumer>();
builder.Services.AddNotifier<ExampleNotifier>();

var app = builder.Build();

app.Run();
