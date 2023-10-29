using Lunacy.Logging.Enums;
using System.Runtime.CompilerServices;

namespace Lunacy.Logging.Extensions {
    public static class ExceptionExtensions {
        public static string Format(this Exception ex) {
            return $"{ex.GetType().Name} : \"{ex.Message}\"\n{ex.StackTrace}";
        }

        public static void LogFast(this Exception ex, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            Logger.LogFast(ex.Format(), severity, LogType.Error, filePath, memberName, lineNumber);
        }
        public static async Task<LogEntry> LogAsync(this Exception ex, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return await Logger.LogAsync(ex.Format(), severity, LogType.Error, filePath, memberName, lineNumber);
        }
        public static LogEntry Log(this Exception ex, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {

            return Logger.Log(ex.Format(), severity, LogType.Error, filePath, memberName, lineNumber);
        }

        public static void LogFast(this Exception ex, string message, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            message += $"\n{ex.Format()}";
            Logger.LogFast(message, severity, LogType.Error, filePath, memberName, lineNumber);
        }
        public static async Task<LogEntry> LogAsync(this Exception ex, string message, LogSeverity severity = LogSeverity.Important,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {

            message += $"\n{ex.Format()}";
            return await Logger.LogAsync(message, severity, LogType.Error, filePath, memberName, lineNumber);
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