using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CFLogGet.Tests
{
    [TestClass]
    public class CFLogParserTest
    {
        [Ignore]
        [TestMethod]
        public void TestMethod1()
        {
            var parser = new CFLogParser();
            var models = parser.Parse(@"C:\Users\nabehiro\dev\CFLogGet\CFLogGet\bin\Debug\temp\logfile.log");
            
            foreach (var p in typeof(CFLogModel).GetProperties())
            {
                var column = p.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                if (column == null) continue;

                if (p.PropertyType != typeof(string)) continue;

                var max = 0;
                foreach (var m in models)
                {
                    var value = (string)p.GetValue(m) ?? "";
                    max = Math.Max(max, value.Length);
                }
                Console.WriteLine($"{p.Name}({column.TypeName}): {max}");
            }
        }
    }
}
