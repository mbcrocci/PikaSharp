using Moq;
using PikaSharp;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pika.Tests;

public class Consumer_Tests
{
    private Mock<IConnector> _connector = new();
    private Mock<IModel> _channel = new();

    [Fact]
    public void Consumer()
    {
        _connector.Setup(c => c.Channel()).Returns(_channel.Object);
    }
}
