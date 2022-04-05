using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public interface IUrlProvider
{
    public string Url { get; }
}

public interface IConnector
{
    IModel Channel();
}

public class Connector : IConnector
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;

    public Connector(IUrlProvider urlProvider)
    {
        _factory = new ConnectionFactory { Uri = new System.Uri(urlProvider.Url) };
        _connection = _factory.CreateConnection();
    }

    public IModel Channel() => _connection.CreateModel();
}
