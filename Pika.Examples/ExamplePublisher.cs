using PikaSharp;

namespace Pika.Examples;

public class ExamplePublisher : Publisher<ExampleMessage>
{
    private readonly PublisherOptions _options = new("tests", "publisher.topic");

    public ExamplePublisher(IConnector rabbit) : base(rabbit) { }

    public override PublisherOptions Options => _options;

    public void CallableMethod(string id, double value)
    {
        Publish(new ExampleMessage(id, value, DateTime.UtcNow));
    }
}
