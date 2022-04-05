using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public record ConsumerOptions(string Exchange, string Topic, string Queue);

public abstract class Consumer<T> : IHostedService
{
    private readonly ILogger<Consumer<T>> _logger;
    private readonly IConnector _rabbit;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public Consumer(ILogger<Consumer<T>> logger, IConnector rabbit)
    {
        _logger = logger;
        _rabbit = rabbit;
    }

    public abstract ConsumerOptions Options { get; }

    public abstract Task ConsumeAsync(T msg);
        
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel ??= _rabbit.Channel();

        SetupQueue();
        SetupConsumer();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!(_channel?.IsClosed ?? true))
            _channel?.Close();

        return Task.CompletedTask;
    }

    private void SetupQueue()
    {
        _channel.QueueDeclare(Options.Queue);
        _channel.QueueBind(Options.Queue, Options.Exchange, Options.Topic);
    }

    private void SetupConsumer()
    {
        _consumer ??= new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            try
            {
                var e = Pika.ReadMessage<T>(ea);
                if (e == null) throw new Exception("Unable to parse message"); // Should only happen on empty/null body

                await ConsumeAsync(e);

                OnSuccess(ea);
            }
            catch (Exception ex)
            {
                OnError(ea, ex);
            }
        };
        _channel.BasicConsume(Options.Queue, false, _consumer);
    }

    private void OnSuccess(BasicDeliverEventArgs ea) => _channel?.BasicAck(ea.DeliveryTag, false);

    private void OnError(BasicDeliverEventArgs ea, Exception e)
    {
        _logger.LogError(e, "Consumer Error");
        _channel?.BasicReject(ea.DeliveryTag, !ea.Redelivered);
        // TODO retries
    }
}
