using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public record ConsumerOptions
{
    public ConsumerOptions(string exchange, string topic, string queue)
    {
        Exchange = exchange;
        Topic = topic;
        Queue = queue;
    }

    public string Exchange { get; set; } = "";
    public string Topic { get; set; } = "";
    public string Queue { get; set; } = "";

    public bool IsDurable { get; internal set; } = false;
    public bool AutoDelete { get; internal set; } = true;
}

public static class ConsumerOptionsExtensions
{
    public static ConsumerOptions Durable(this ConsumerOptions options)
    {
        options.IsDurable = true;
        options.AutoDelete = false;
        return options;
    }
}

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

    protected ILogger Logger { get { return _logger; } }

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
        _channel.QueueDeclare(Options.Queue, Options.IsDurable, false, Options.AutoDelete);
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
