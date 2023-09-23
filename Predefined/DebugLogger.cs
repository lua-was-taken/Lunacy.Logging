using Lunacy.Logging.Interfaces;
using System.Diagnostics;

namespace Lunacy.Logging.Predefined {
    public class DebugLogger : ILogger {
        public void Handle(LogEntry entry) {
            Debug.WriteLine(entry.ToString());
        }
    }
}