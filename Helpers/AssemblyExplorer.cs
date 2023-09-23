
using System.Diagnostics;
using System.Reflection;

namespace Lunacy.Logging.Helpers {
    internal static class AssemblyExplorer {

        public readonly struct CallerInfo {
            public static readonly CallerInfo Unknown = new() {
                DeclaringTypeFullName = string.Empty,
                AssemblyName = string.Empty,
                MethodName = string.Empty,
                AssemblyVer = string.Empty
            };

            public required string DeclaringTypeFullName { get; init; }
            public required string AssemblyName { get; init; }
            public required string MethodName { get; init; }
            public required string AssemblyVer { get; init; }
        }

        public static FileVersionInfo? GetCallerFileMetaData(bool skipThisAssembly = true) {
            StackTrace trace = new();
            (MethodBase method, Type type)? caller = GetExternalCaller(trace.GetFrames(), skipThisAssembly);

            if(!caller.HasValue) {
                return default;
            }

            Assembly callerAsm = caller.Value.type.Assembly;
            return FileVersionInfo.GetVersionInfo(callerAsm.Location);
        }

        public static CallerInfo GetCallerInfo(bool skipThisAssembly = true) {
            StackTrace trace = new();
            (MethodBase method, Type type)? caller = GetExternalCaller(trace.GetFrames(), skipThisAssembly);

            if(!caller.HasValue) {
                return CallerInfo.Unknown;
            }

            Assembly callerAsm = caller.Value.type.Assembly;

            string asmVersion = callerAsm.GetName().Version?.ToString() ?? string.Empty;
            string asmName = callerAsm.GetName().Name ?? string.Empty;

            string methodName = caller.Value.method.Name;
            string declFullName = caller.Value.type.FullName ?? string.Empty;

            if(string.IsNullOrWhiteSpace(asmVersion) || string.IsNullOrWhiteSpace(asmName) || string.IsNullOrWhiteSpace(methodName) || string.IsNullOrWhiteSpace(declFullName)) {
                return CallerInfo.Unknown;
            } else {
                return new CallerInfo() {
                    AssemblyName = asmName,
                    AssemblyVer = asmVersion,
                    MethodName = methodName,
                    DeclaringTypeFullName = declFullName
                };
            }
        }

        private static (MethodBase method, Type type)? GetExternalCaller(StackFrame[] frames, bool skipThisAssembly) {
            Assembly thisAsm = Assembly.GetExecutingAssembly();

            foreach (StackFrame frame in frames) {

                MethodBase? method = frame.GetMethod();
                Type? declaringType = method?.DeclaringType;

                if(method == default || declaringType == default) {
                    continue;
                }

                if(declaringType.Assembly.Location != thisAsm.Location && skipThisAssembly) {
                    return (method, declaringType);
                }

            }

            return default;
        }
    }
}