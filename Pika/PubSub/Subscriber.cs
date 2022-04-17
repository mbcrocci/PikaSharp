using System.Collections.Concurrent;

namespace PikaSharp.PubSub;

public interface ISubscriber
{
    Task OnMessage<T>(T msg);
}

public class Subscriber<T>
{
    private readonly ISubscriber _subscriber;
    private readonly BlockingCollection<T> _queue;

    public Subscriber(ISubscriber subscriber)
    {
        _subscriber = subscriber;
        _queue = new();
    }

    public void Publish(T msg)
    {
        _queue.Add(msg);
    }
}