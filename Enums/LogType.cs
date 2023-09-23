using System.ComponentModel;

namespace Lunacy.Logging.Enums {
    public enum LogType {
        [Description("INFO")]
        Info,
        [Description("WARN")]
        Warning,
        [Description("ERR ")]
        Error
    }
}