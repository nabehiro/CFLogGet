using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    public abstract class CommandBase
    {
        public void OnExecute(CommandLineApplication app)
        {
            Console.WriteLine("Parameters:");

            foreach (var opt in app.GetOptions().Where(o => o.LongName != "help"))
                Console.WriteLine($"\t{opt.LongName}: {opt.Value()}");

            Console.WriteLine();

            Execute();
        }

        public abstract void Execute();
    }
}
