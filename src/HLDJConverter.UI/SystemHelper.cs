using System;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;

namespace HLDJConverter.UI
{
    public static class SystemHelper
    {
        private static string GetRegistryString(string path, string key)
        {
            try
            {
                var registryKey = Registry.LocalMachine.OpenSubKey(path);
                return registryKey?.GetValue(key).ToString() ?? string.Empty;
            }
            catch { return string.Empty; }
        }

        public static string GetOperatingSystem()
        {
            string productName = GetRegistryString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName").Trim(' ');
            string csdVersion = GetRegistryString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion").Trim(' ');
            string architexture = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            if(!string.IsNullOrEmpty(productName))
            {
                if(!productName.StartsWith("Microsoft"))
                    productName = $"Microsoft {productName}";
                
                return JoinNotEmptyStrings(" ", productName, csdVersion, architexture);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetLanguage()
        {
            return CultureInfo.InstalledUICulture.EnglishName;
        }

        /// <summary>
        /// Does a string.Join but only on elements that aren't null or empty.
        /// </summary>
        private static string JoinNotEmptyStrings(string seperator, params string[] values)
        {
            return string.Join(seperator, values.Where(str => !string.IsNullOrWhiteSpace(str)));
        }
    }
}
