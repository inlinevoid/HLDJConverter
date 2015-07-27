using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            HookAssemblyResolver();
            App.Main();
        }

        public static void HookAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                if(assemblyName.Contains(".resources"))
                {
                    Console.WriteLine("AssemblyResolve: {0}", assemblyName);
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
            };
        }
    }
}
