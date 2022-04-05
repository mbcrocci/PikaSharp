using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PikaSharp;

public static class Pika
{
    public static byte[] Message<T>(T obj) => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);
    public static T? ReadMessage<T>(byte[] body) => System.Text.Json.JsonSerializer.Deserialize<T>(body);
    public static T? ReadMessage<T>(BasicDeliverEventArgs eventArgs) => ReadMessage<T>(eventArgs.Body.ToArray());

    public static string AppendToKey(string key, string str)
    {
        var strKey = str
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("-", "")
            .Replace("_", "")
            .ToLower();

        return $"{key}.{strKey}";
    }

    public static IBasicProperties PersistentProperties(IModel channel)
    {
        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        return props;
    }
}
