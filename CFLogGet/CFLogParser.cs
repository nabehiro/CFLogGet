using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    public class CFLogParser
    {
        public IEnumerable<CFLogModel> Parse(string filePath)
        {
            var list = new List<CFLogModel>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                var values = line.Split('\t');

                var model = new CFLogModel();
                list.Add(model);

                foreach (var prop in model.GetType().GetProperties().Where(p => p.CanWrite))
                {
                    var column = GetAttribute<ColumnAttribute>(prop);
                    if (column == null) continue;

                    var value = values[column.Order];
                    if (string.IsNullOrEmpty(value) || value == "-") continue;

                    var stringLength = GetAttribute<StringLengthAttribute>(prop);
                    if (stringLength != null && stringLength.MaximumLength < value.Length)
                        value = value.Substring(0, stringLength.MaximumLength - 3) + "...";

                    if (prop.PropertyType == typeof(int?))
                        prop.SetValue(model, int.Parse(value));
                    else if (prop.PropertyType == typeof(decimal?))
                        prop.SetValue(model, decimal.Parse(value));
                    else
                        prop.SetValue(model, value);
                }
            }
            return list;
        }

        private TAttribute GetAttribute<TAttribute>(PropertyInfo prop) where TAttribute : class
            => prop.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
    }
}
