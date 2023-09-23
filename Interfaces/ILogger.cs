namespace Lunacy.Logging.Interfaces
{
    public interface ILogger {
        public void Handle(LogEntry entry);
    }
}