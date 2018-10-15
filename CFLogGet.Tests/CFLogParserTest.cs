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
            var models = parser.Parse(@"C:\tools\cflogget\temp\commerble-corp\logfile.log");
            
            foreach (var p in typeof(CFLogModel).GetProperties())
            {
                var column = p.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                if (column == null) continue;

                if (p.PropertyType != typeof(string)) continue;

                var maxValue = "";
                var maxLength = 0;
                foreach (var m in models)
                {
                    var value = (string)p.GetValue(m) ?? "";
                    if (value.Length > maxLength)
                    {
                        maxLength = value.Length;
                        maxValue = value;
                    }
                }
                Console.WriteLine($"** {p.Name}({column.TypeName}): {maxLength}, {maxValue}\n");
            }
        }
    }
}
