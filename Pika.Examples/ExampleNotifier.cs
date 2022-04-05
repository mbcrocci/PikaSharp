using PikaSharp;

namespace Pika.Examples;

public class ExampleNotifier : Notifier<ExampleMessage>
{
    private readonly NotifierOptions _options = new();

    public ExampleNotifier(IConnector connector) : base(connector) { }

    public override NotifierOptions Options => _options;

    public override ExampleMessage Notify()
    {
        return new ExampleMessage("id123", 10.0, DateTime.UtcNow);
    }
}
