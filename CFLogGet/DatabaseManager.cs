using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace CFLogGet
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTable(string tableName)
        {
            var sql = new StringBuilder();

            // CREATE TABLE
            sql.AppendLine($"CREATE TABLE [{tableName}] (");

            foreach (var prop in typeof(CFLogModel).GetProperties())
            {
                var column = prop.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                if (column == null) continue;

                sql.AppendLine($"\t[{prop.Name}] {column.TypeName} NULL,");
            }
            sql.Remove(sql.Length - 1, 1);

            sql.AppendLine(");");

            // CREATE INDEX
            sql.AppendLine($"CREATE CLUSTERED INDEX CLS_IDX_dt_{tableName} ON {tableName} (dt);");
            sql.AppendLine($"CREATE INDEX IDX_cs_uri_stem_{tableName} ON {tableName} (cs_uri_stem)");
            sql.AppendLine($"CREATE INDEX IDX_c_ip_{tableName} ON {tableName} (c_ip)");
            sql.AppendLine($"CREATE INDEX IDX_sc_status_{tableName} ON {tableName} (sc_status)");

            using (var con = new SqlConnection(_connectionString))
            {
                //Console.WriteLine(sql.ToString());
                con.Execute(sql.ToString());
            }
        }

        public void Insert(string tableName, IEnumerable<CFLogModel> models)
        {
            var table = new DataTable(tableName);
            var props = typeof(CFLogModel).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() is ColumnAttribute)
                .ToList();

            props.ForEach(p => table.Columns.Add(p.Name));

            foreach (var model in models)
            {
                var row = table.NewRow();
                props.ForEach(p => row[p.Name] = p.GetValue(model));
                table.Rows.Add(row);
            }

            using (var bulkCopy = new SqlBulkCopy(_connectionString))
            {
                props.ForEach(p => bulkCopy.ColumnMappings.Add(p.Name, p.Name));

                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.WriteToServer(table);
            }
        }

        public int Count(string tableName)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                return con.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName}");
            }
        }
    }
}
