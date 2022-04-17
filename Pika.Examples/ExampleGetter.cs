using PikaSharp;

namespace Pika.Examples;

public class ExampleGetter : Getter<ExampleMessage, ExampleMessage>
{
    public ExampleGetter(
        ILogger<Getter<ExampleMessage, ExampleMessage>>
        logger, IConnector rabbit) : base(logger, rabbit)
    {
    }
}

public class ExampleGetterUser
{
    private readonly ILogger<ExampleGetterUser> _logger;
    private readonly ExampleGetter _getter;

    public ExampleGetterUser(
        ILogger<ExampleGetterUser> logger,
        ExampleGetter getter)
    {
        _logger = logger;
        _getter = getter;
    }

    public void Work()
    {
        try
        {
            var msg = _getter.Get(
                new ExampleMessage("", 0.0, DateTime.UtcNow),
                new GetterOptions("getter.topic"));

            _logger.LogInformation("{@ExampleMessage}", msg);
        }
        catch (Exception)
        {
            // handle exception
        }
    }
}