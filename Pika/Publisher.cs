using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikaSharp;

public record PublisherOptions(
    string Exchange,
    string Topic,
    IBasicProperties? props = null);

public abstract class Publisher<T>
{
    private readonly IConnector _rabbit;
    private readonly IModel _channel;

    public Publisher(IConnector rabbit)
    {
        _rabbit = rabbit;
        _channel = _rabbit.Channel();
    }

    public abstract PublisherOptions Options { get; }

    protected void Publish(T message) => Publish(message, Options);

    protected void Publish(T message, PublisherOptions options)
    {
        _channel.BasicPublish(
            options.Exchange,
            options.Topic,
            basicProperties: options.props,
            body: Pika.Message(message));
    }

    protected IBasicProperties CreateProperties() => _channel.CreateBasicProperties();
}