using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public interface IConnector
{
    IModel Channel();
}

public class RabbitConnector : IConnector
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;

    public RabbitConnector(string url)
    {
        _factory = new ConnectionFactory { Uri = new System.Uri(url) };
        _connection = _factory.CreateConnection();
    }

    public IModel Channel() => _connection.CreateModel();
}
