using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikaSharp;

public class NotifierOptions
{
    public string Exchange { get; set; } = "";
    public string Topic { get; set; } = "";
    public TimeSpan Interval { get; set; }
}

public abstract class Notifier<T> : BackgroundService
{
    private readonly RabbitConnector _connector;
    private IModel? _channel;

    public Notifier(RabbitConnector connector)
    {
        _connector = connector;
    }

    public abstract NotifierOptions Options { get; }
    public abstract T Notifify();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
        {
            _channel = _connector.Channel();
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var msg = Notifify();
            SendNotification(msg, Options);

            await Task.Delay(Options.Interval, cancellationToken);
        }
    }

    private void SendNotification(T message, NotifierOptions options)
    {
        if (message == null) return;

        _channel.BasicPublish(options.Exchange, options.Topic, body: Pika.Message(message));
    }
}
