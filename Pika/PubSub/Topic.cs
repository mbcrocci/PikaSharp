namespace PikaSharp.PubSub;

public interface ITopic
{
    void Subscribe(ISubscriber subscriber);
    void Broadcast<T>(T msg);
}

public class Topic : ITopic
{
    private readonly List<ISubscriber> _subscribers = new();

    public void Subscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Broadcast<T>(T msg)
    {
        foreach (var s in _subscribers)
        {
            s.OnMessage(msg);
        }
    }
}