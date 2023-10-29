using Lunacy.Logging.Enums;
using Lunacy.Logging.Extensions;
using Lunacy.Logging.Helpers;
using Lunacy.Logging.Interfaces;
using System.Diagnostics;

namespace Lunacy.Logging.Predefined {
    public class FileLogger : ILogger {

        /// <summary>
        /// The available symbols to use when defining a name format
        /// </summary>
        public const string AvailableSymbols = "%r = Replace with random ID | %sr = Replace with short random ID | %t = Replace with time";

        public string Location { get; protected init; }
        public string NameFormat { get; protected init; }

        public bool AutoCreateDirectory { get; set; } = true;
        public bool AutoDeleteLogs { get; set; } = false;

        public int MaxLogCount { get; set; } = 50;
        public TimeSpan MaxLogAge { get; set; } = TimeSpan.FromDays(30);
        public long MaxLogFileSize { get; set; } = 10_485_760L; //10MB

        public string ActiveFilePath { get; protected set; } = string.Empty;

        public FileLogger() : this(string.Empty, string.Empty) { }
        public FileLogger(string location) : this(location, string.Empty) { }
        public FileLogger(string location, string nameFormat) {

            string productName = GetProductName();
            
            if(string.IsNullOrWhiteSpace(location)) {
                Location = Path.GetTempPath();
                if(!string.IsNullOrWhiteSpace(productName)) {
                    Location += productName + '\\';
                }
            } else {
                Location = location;
            }

            if(string.IsNullOrWhiteSpace(nameFormat)) {

                string format = productName;
                if(string.IsNullOrWhiteSpace(format)) {
                    format = "%r";
                }

                NameFormat = $"{format}@%t.%sr.log";
            } else {
                NameFormat = nameFormat;
            }
        }

        protected static string GetProductName() {
            FileVersionInfo? metaData = AssemblyExplorer.GetCallerFileMetaData();
            string? productName = string.Empty;

            if(metaData != default) {
                productName = metaData.ProductName ?? metaData.InternalName;
            }

            return productName ?? AssemblyExplorer.GetCallerInfo().AssemblyName;
        }

        protected volatile int counter = 0;
        protected readonly object _lock = new();
        public void Handle(LogEntry entry) {
            lock(_lock) {
                try {

                    if(string.IsNullOrWhiteSpace(ActiveFilePath)) {
                        ActiveFilePath = CreateLogFile();
                    }

                    FileInfo fInfo = new(ActiveFilePath);
                    if(fInfo.Length >= MaxLogFileSize) {

                        ActiveFilePath = CreateLogFile();
                        fInfo = new(ActiveFilePath);

                    }

                    if(AutoDeleteLogs) {
                        if(counter == 0) {
                            try {
                                DeleteLogs();
                            } catch(Exception ex) {
                                Logger.Disable(this);
                                ex.Log();
                                Logger.Enable(this);
                            }
                            counter++;
                        } else if(counter >= 100) {
                            counter = 0;
                        } else {
                            counter++;
                        }
                    }

                    File.AppendAllText(ActiveFilePath, entry.ToString() + '\n');

                } catch(Exception ex) {
                    Logger.Disable(this);
                    ex.Log(LogSeverity.Critical);
                    Logger.Enable(this);
                }
            }
        }

        public void DeleteLogs() {
            if(!AutoDeleteLogs) {
                throw new InvalidOperationException("Auto delete logs is not enabled.");
            }

            IOrderedEnumerable<FileInfo> logFiles = Directory.GetFiles(Location)
                .Where(x => IsFormatted(Path.GetFileName(x), NameFormat))
                .Select(x => new FileInfo(x))
                .OrderByDescending(x => x.LastWriteTime);

            if(logFiles.Count() > MaxLogCount) {
                IEnumerable<FileInfo> oldFiles = logFiles.Reverse().Take(logFiles.Count() - MaxLogCount);
                foreach(FileInfo oldFile in oldFiles) {
                    try {

                        oldFile.Attributes = FileAttributes.Normal;
                        oldFile.IsReadOnly = false;
                        oldFile.Delete();

                    } catch(Exception ex) {
                        Logger.Disable(this);
                        ex.Log();
                        Logger.Enable(this);
                    }
                }
            }

            foreach(FileInfo fInfo in logFiles) {
                try {

                    fInfo.Refresh();
                    if(!fInfo.Exists) {
                        continue;
                    }

                    if(DateTime.Now - fInfo.LastWriteTime >= MaxLogAge) {
                        fInfo.Attributes = FileAttributes.Normal;
                        fInfo.IsReadOnly = false;
                        fInfo.Delete();

                        continue;
                    }

                } catch(Exception ex) {
                    Logger.Disable(this);
                    ex.Log();
                    Logger.Enable(this);
                }
            }
        }

        protected static bool IsFormatted(string input, string format) {
            if(input.Length < format.Length) {
                return false;
            }

            int x = 0;
            for(int i = 0; i < format.Length; i++) {

                if(format[i] == '%' && format.Length - 1 >= i + 1) {
                    string symbol = format[i..(i + 2)];
                    if(symbol == "%s" && format.Length - 1 >= i + 2) {
                        symbol = format[i..(i + 3)];
                    }

                    switch(symbol.ToLowerInvariant()) {

                        case "%r":
                            x += 16 - 1;
                            i += 2 - 1;
                            break;

                        case "%sr":
                            x += 8 - 1;
                            i += 3 - 1;
                            break;

                        case "%t":
                            x += 12 - 1;
                            i += 2 - 1;
                            break;

                        default:
                            if(input[x] != '%') {
                                return false;
                            }
                            break;
                    }

                } else {
                    if(input[x] != format[i]) {
                        return false;
                    }
                }

                x++;

            }

            //note: equal due to x being incremented regardless of for statement condition
            return input.Length == x;
        }

        protected virtual string CreateLogFile() {
            if(!Directory.Exists(Location)) {
                if(AutoCreateDirectory) {
                    Directory.CreateDirectory(Location);
                } else {
                    throw new DirectoryNotFoundException(Location);
                }
            }

            string fileName = NameFormat;

            int index;
            while((index = fileName.IndexOf("%r")) != -1) {
                fileName = fileName[..index] + CreateRandomID(16) + fileName[(index + "%r".Length)..];
            }

            while((index = fileName.IndexOf("%sr")) != -1) {
                fileName = fileName[..index] + CreateRandomID(8) + fileName[(index + "%sr".Length)..];
            }

            DateTime time = DateTime.Now;
            while((index = fileName.IndexOf("%t")) != -1) {
                fileName = fileName[..index] + $"{time:yy}{time:MM}{time:dd}{time:HH}{time:mm}{time:ss}" + fileName[(index + "%t".Length)..];
            }

            fileName = Path.Combine(Location, fileName);
            File.WriteAllBytes(fileName, Array.Empty<byte>()); //Create file

            return fileName;
        }

        protected static string CreateRandomID(int length) {
            byte[] buffer = new byte[(int)Math.Ceiling(length / 2D)];
            Random.Shared.NextBytes(buffer);

            return string.Join(string.Empty, buffer.Select(x => x.ToString("X2")))[..length];
        }
    }
}