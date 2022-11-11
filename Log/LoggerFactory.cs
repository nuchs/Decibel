namespace Log;

public static class LoggerFactory
{
    public static ILog CreateLogger<T>()
    {
        var t = typeof(T);

        return new Log(t.Name);
    }
}
