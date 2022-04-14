using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikaSharp;

public record PublisherOptions(string Exchange, string Topic);

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

    protected void Publish(T message)
    {
        _channel.BasicPublish(Options.Exchange, Options.Topic, body: Pika.Message(message));
    }

    protected void Publish(T message, PublisherOptions options)
    {
        _channel.BasicPublish(options.Exchange, options.Topic, body: Pika.Message(message));
    }
}