using System.Runtime.CompilerServices;

namespace Log;

public interface ILog
{
    void Debug(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0);

    void Error(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0);

    void Info(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0);

    void Warn(string msg, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0);
}
