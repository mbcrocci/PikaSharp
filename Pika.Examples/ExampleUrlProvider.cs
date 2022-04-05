using PikaSharp;

namespace Pika.Examples;

public class ExampleUrlProvider : IUrlProvider
{
    public string Url => "amqp://localhost:5672"; // You could get it from configuration for example
}
