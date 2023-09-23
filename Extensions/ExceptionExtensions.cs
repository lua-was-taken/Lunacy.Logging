using Lunacy.Logging.Enums;
using System.Runtime.CompilerServices;

namespace Lunacy.Logging.Extensions
{
    public static class ExceptionExtensions {
        public static string Format(this Exception ex) {
            return $"{ex.GetType().Name} : \"{ex.Message}\"\n{ex.StackTrace}";
        }

        public static LogEntry Log(this Exception ex, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default, 
            [CallerMemberName] string? memberName = default, 
            [CallerLineNumber] int lineNumber = -1) {

            return Logger.Log(ex.Format(), severity, LogType.Error, filePath, memberName, lineNumber);
        }

        public static LogEntry Log(this Exception ex, string message, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {

            message += $"\n{ex.Format()}";
            return Logger.Log(message, severity, LogType.Error, filePath, memberName, lineNumber);
        }
    }
}