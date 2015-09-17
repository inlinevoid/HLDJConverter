using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.UI
{
    public static class DebugHelper
    {
        public static void HandleCrashFromUnhandledException(UnhandledExceptionEventArgs args)
        {
            const string CrashLogFilename = "HLDJConverterCrashLog.txt";
            string crashLog = GenerateCrashLog(args.ExceptionObject.ToString());
            
            var gistResponse = Gist.CreateAnonymousGist("Auto-generated crash report created by https://github.com/inlinevoid/HLDJConverter", false, CrashLogFilename, crashLog);
            
            crashLog = GenerateCrashLogHeaderForGist(gistResponse) + crashLog;

            File.WriteAllText(CrashLogFilename, crashLog);
        }

        public static string GenerateCrashLog(string stackTrace)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"HLDJConverter version: {ApplicationHelper.GetVersion()}");
            sb.AppendLine($"Operating system: {SystemHelper.GetOperatingSystem()}");
            sb.AppendLine($"Language: {SystemHelper.GetLanguage()}");
            sb.AppendLine();
            sb.AppendLine(stackTrace);

            return sb.ToString();
        }
        

        public static string GenerateCrashLogHeaderForGist(GistSubmissionResponse submissionResponse)
        {
            if(submissionResponse == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"========= SEND THIS LINK TO INLINEVOID =========");
            sb.Append('\n', 2);
            sb.AppendLine(submissionResponse.Url);
            sb.Append('\n', 3);
            sb.AppendLine($"========= CRASH LOG =========");

            return sb.ToString();
        }
    }
}
