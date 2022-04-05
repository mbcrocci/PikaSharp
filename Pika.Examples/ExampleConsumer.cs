using PikaSharp;

namespace Pika.Examples;

public class ExampleConsumer : Consumer<ExampleMessage>
{
    private readonly ConsumerOptions _options = new("tests", "example.topic", "example-consumer");

    public ExampleConsumer(ILogger<ExampleConsumer> logger, IConnector connector) : base(logger, connector) { }

    public override ConsumerOptions Options => _options;

    public override async Task ConsumeAsync(ExampleMessage msg)
    {
        Logger.LogInformation($"{msg.TimeStamp} - {msg.Id} {msg.Value}");
        await Task.Delay(1000);
    }
}
