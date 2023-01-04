using System.Runtime.CompilerServices;

namespace Log;

internal class Log : ILog
{
    private readonly string type;

    internal Log(string type) => this.type = type;

    public void Debug(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0)
        => Write(msg, method, line);

    public void Error(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0)
        => Write(msg, method, line);

    public void Info(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0)
        => Write(msg, method, line);

    public void Warn(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0)
        => Write(msg, method, line);

    private void Write(string msg, string method, int line, [CallerMemberName] string level = "")
    {
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {level} | {type}.{method} {line} | {msg}");
    }
}
