using Lunacy.Logging.Enums;
using System.Runtime.CompilerServices;

namespace Lunacy.Logging.Extensions {
    public static class StringExtensions {
        public static LogEntry Log(this string message, LogSeverity severity = LogSeverity.Normal, LogType type = LogType.Info,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return Logger.Log(message, severity, type, filePath, memberName, lineNumber);
        }

        public static LogEntry LogError(this string message, LogSeverity severity = LogSeverity.Normal,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return Logger.Log(message, severity, LogType.Error, filePath, memberName, lineNumber);
        }

        public static LogEntry LogInfo(this string message, LogSeverity severity = LogSeverity.Normal,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return Logger.Log(message, severity, LogType.Info, filePath, memberName, lineNumber);
        }

        public static LogEntry LogWarn(this string message, LogSeverity severity = LogSeverity.Normal,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return Logger.Log(message, severity, LogType.Warning, filePath, memberName, lineNumber);
        }

        public static LogEntry LogVerbose(this string message, LogType type = LogType.Info,
            [CallerFilePath] string? filePath = default,
            [CallerMemberName] string? memberName = default,
            [CallerLineNumber] int lineNumber = -1) {
            return Logger.Log(message, LogSeverity.Verbose, type, filePath, memberName, lineNumber);
        }
    }
}