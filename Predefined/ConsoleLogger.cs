using Lunacy.Logging.Enums;
using Lunacy.Logging.Interfaces;

namespace Lunacy.Logging.Predefined {
    public class ConsoleLogger : ILogger {
        public void Handle(LogEntry entry) {
            ConsoleColor color = Console.ForegroundColor;
            
            switch(entry.Type) {

                case LogType.Error: 
                    if(entry.Severity == LogSeverity.Critical) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    break;

                case LogType.Warning:
                    if(entry.Severity == LogSeverity.Critical) {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.WriteLine(entry.ToString());
            Console.ForegroundColor = color;
        }
    }
}