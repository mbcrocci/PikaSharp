using System.Collections.Concurrent;

namespace PikaSharp.PubSub;

public class PubSub
{
    private readonly ConcurrentDictionary<string, Topic> _topics = new();

    public void Subscribe(string topic, ISubscriber subscriber)
    {
        _topics.TryAdd(topic, new Topic());
        _topics[topic].Subscribe(subscriber);
    }

    public void Broadcast<T>(string topic, T msg)
    {
        if (_topics.TryGetValue(topic, out var t))
        {
            t.Broadcast(msg);
        }
    }
}