using Lunacy.Logging.Enums;

namespace Lunacy.Logging
{
    public class LogEntry
    {
        public static readonly LogEntry Empty = new()
        {
            Severity = LogSeverity.Normal,
            Type = LogType.Info,

            SourceFile = string.Empty,
            SourceMember = string.Empty,
            SourceAssembly = string.Empty,
            SourceLineNumber = -1,

            LogTime = DateTime.MinValue,
            Message = string.Empty,
        };

        public required string Message { get; init; } = string.Empty;
        public required LogSeverity Severity { get; init; } = LogSeverity.Normal;
        public required LogType Type { get; init; } = LogType.Info;

        public required DateTime LogTime { get; init; } = DateTime.MinValue;

        public required string SourceAssembly { get; init; } = string.Empty;
        public required string SourceFile { get; init; } = string.Empty;
        public required string SourceMember { get; init; } = string.Empty;
        public required int SourceLineNumber { get; init; } = -1;


        public string Source
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SourceFile) || string.IsNullOrWhiteSpace(SourceAssembly)
                || string.IsNullOrWhiteSpace(SourceMember) || SourceLineNumber == -1)
                {
                    return "UNKN";
                }

                string sourceFile = SourceFile;
                if (sourceFile.Contains(SourceAssembly))
                {
                    sourceFile = sourceFile[sourceFile.IndexOf(SourceAssembly)..];
                }

                return $"{sourceFile}@{SourceMember}:{SourceLineNumber}";
            }
        }

        public LogEntry() { }

        public override string ToString()
        {
            string strSeverity = Enum.GetName(Severity) ?? "UNKN";
            string strType = Enum.GetName(Type) ?? "UNKN";

            string time = $"{LogTime:yy}:{LogTime:MM}:{LogTime:dd}:{LogTime:HH}:{LogTime:mm}:{LogTime:ss}";
            return $"[{strSeverity} {strType} @ {time}] {Source} :: {Message}";
        }
    }
}