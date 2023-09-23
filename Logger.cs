using Lunacy.Logging.Enums;
using Lunacy.Logging.Helpers;
using Lunacy.Logging.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lunacy.Logging
{
    public static class Logger {

        private static readonly ConcurrentDictionary<ILogger, bool> Handlers = new();
        private static volatile bool Enabled = false;

        public static LogEntry Log(string message, LogSeverity severity = LogSeverity.Normal, LogType type = LogType.Info, 
            [CallerFilePath] string? filePath = default, 
            [CallerMemberName] string? memberName = default, 
            [CallerLineNumber] int lineNumber = 0) {

            filePath ??= "UNKN";
            memberName ??= "UNKN";

            DateTime logTime = DateTime.Now;

            AssemblyExplorer.CallerInfo callerInfo = AssemblyExplorer.GetCallerInfo(skipThisAssembly: true);
            if(callerInfo.Equals(AssemblyExplorer.CallerInfo.Unknown)) {

                Debug.WriteLine($"Fatal logging error:: Cannot evaluate stack trace, log message: \"{message}\"");
                return LogEntry.Empty;
            }

            LogEntry entry = new() {
                Severity = severity,
                Type = type,
                
                SourceAssembly = callerInfo.AssemblyName,
                SourceFile = filePath,
                SourceMember = memberName,
                SourceLineNumber = lineNumber,
                LogTime = logTime,

                Message = message
            };

            foreach((ILogger logger, bool enabled) in Handlers) {
                if(enabled) {
                    logger.Handle(entry);
                }
            }

            return entry;

        }

        public static bool AddLogger(ILogger logger) {
            return Handlers.TryAdd(logger, true);
        }

        public static bool AddLogger<T>() where T : ILogger, new() {
            foreach((ILogger logger, bool _) in Handlers) {
                if(logger.GetType().IsEquivalentTo(typeof(T))) {
                    throw new InvalidOperationException("A logger of the same type has already been added. " +
                        "If this was intentional, please use the explicit AddLogger method.");
                }
            }

            return Handlers.TryAdd(new T(), true);
        }

        public static T? GetLogger<T>() where T : ILogger, new() {
            foreach((ILogger logger, bool _) in Handlers) {
                if(logger.GetType().IsEquivalentTo(typeof(T))) {
                    return (T)logger;
                }
            }

            return default;
        }

        public static bool RemoveLogger<T>() where T : ILogger, new() {
            foreach((ILogger logger, bool _) in Handlers.ToList()) {
                if(logger.GetType().IsEquivalentTo(typeof(T))) {
                    return Handlers.Remove(logger, out _);
                }
            }

            return false;
        }

        public static bool RemoveLogger(ILogger logger) {
            return Handlers.TryRemove(logger, out _);
        }

        public static bool Enable() {
            if(Enabled) {
                return false;
            }

            return Enabled = true;
        }

        public static bool Enable(ILogger logger) {
            if(Handlers.ContainsKey(logger)) {
                bool enabled = Handlers[logger];
                if(enabled) {
                    return false;
                }

                return Handlers[logger] = true;
            }

            return false;
        }

        public static bool Disable() {
            if(!Enabled) {
                return false;
            }

            Enabled = false;
            return true;
        }

        public static bool Disable(ILogger logger) {
            if(Handlers.ContainsKey(logger)) {
                bool enabled = Handlers[logger];
                if(!enabled) {
                    return false;
                }

                return Handlers[logger] = false;
            }

            return false;
        }
    }
}