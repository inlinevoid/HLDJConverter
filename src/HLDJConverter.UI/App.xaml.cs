using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace HLDJConverter.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        
    }

    public sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            App.Main();
        }

        public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            DebugHelper.HandleCrashFromUnhandledException(args);
        }

        public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            if(assemblyName.Contains(".resources"))
            {
                //Console.WriteLine("AssemblyResolve: {0}", assemblyName);
                return null;
            }

            string fullAssemblyName = string.Empty;
            foreach(var manifestResource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if(manifestResource.Contains(assemblyName))
                {
                    fullAssemblyName = manifestResource;
                    break;
                }
            }

            if(fullAssemblyName == string.Empty)
                return null;

            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullAssemblyName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
