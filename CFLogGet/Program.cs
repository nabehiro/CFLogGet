using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    [Command("cflogget.exe")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand("get", typeof(GetLogsCommand))]
    [Subcommand("register", typeof(RegisterProfileCommand))]
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

        private static string GetVersion()
            => $"cflogget ver{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";

        private int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 1;
        }
    }
}
