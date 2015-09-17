using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.UI
{
    public static class ApplicationHelper
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"{version.ProductMajorPart}.{version.ProductMinorPart}";
        }
    }
}
