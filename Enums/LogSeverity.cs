using System.ComponentModel;

namespace Lunacy.Logging.Enums {
    public enum LogSeverity {
        [Description("VERB")]
        Verbose,
        [Description("NORM")]
        Normal,
        [Description("IMPT")]
        Important,
        [Description("CRIT")]
        Critical
    }
}