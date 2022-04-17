using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public record GetterOptions(string Topic);

public abstract class Getter<T, R> : IHostedService
{
    private readonly ILogger<Getter<T, R>> _logger;

    private readonly IConnector _rabbit;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    private readonly string _correlationId;
    private readonly BlockingCollection<R> _queue;

    public Getter(ILogger<Getter<T, R>> logger, IConnector rabbit)
    {
        _logger = logger;
        _rabbit = rabbit;

        _correlationId = Guid.NewGuid().ToString();
        _queue = new();
    }

    public R Get(T msg, GetterOptions options)
    {
        if (_channel == null)
            throw new Exception("Can't Get on a null channel");

        var props = _channel.CreateBasicProperties();
        props.CorrelationId = _correlationId;
        props.ReplyTo = "amq.rabbitmq.reply-to";

        _channel.BasicPublish(
            exchange: "",
            routingKey: options.Topic,
            basicProperties: props,
            body: Pika.Message(msg));

        return _queue.Take();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer ??= new EventingBasicConsumer(_channel);
        _consumer.Received += (model, ea) =>
        {
            try
            {
                var e = Pika.ReadMessage<R>(ea);
                if (e == null) throw new Exception("Unable to parse message"); // Should only happen on empty/null body

                if (ea.BasicProperties.CorrelationId == _correlationId)
                {
                    _queue.Add(e);
                }

                OnSuccess(ea);
            }
            catch (Exception ex)
            {
                OnError(ea, ex);
            }
        };
        _channel.BasicConsume("", false, _consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null && _channel.IsOpen)
            _channel.Close();

        return Task.CompletedTask;
    }

    private void OnSuccess(BasicDeliverEventArgs ea) => _channel?.BasicAck(ea.DeliveryTag, false);

    private void OnError(BasicDeliverEventArgs ea, Exception e)
    {
        _logger.LogError(e, "Consumer Error");
        _channel?.BasicReject(ea.DeliveryTag, !ea.Redelivered);
        // TODO retries
    }
}
