using Amazon.S3.Model;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    [Command("get" , Description = "Download CloudFront logs and store logs on database.")]
    public class GetLogsCommand : CommandBase
    {
        [Required]
        [Option(Description = "Required. S3 Bucket name")]
        public string BucketName { get; set; }

        [Required]
        [Option(Description = "Required. Root directory(e.g. commerble-corp)")]
        public string RootDirectory { get; set; }

        [Required]
        [DateTimeString]
        [Option(Description = "Required. start datetime to get logs(e.g. 2018-01-01 10:00)")]
        public string StartDateTime { get; set; }

        [Required]
        [DateTimeString]
        [Option(Description = "Required. end datetime to get logs(e.g. 2018-01-01 10:00)")]
        public string EndDateTime { get; set; }

        private S3Accessor _s3;
        private CFLogParser _logParser;
        private DatabaseManager _database;
        private string _tableName;

        public override void Execute()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["default"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new ApplicationException("ConnectionString must be required in configuration file.");

            _database = new DatabaseManager(connectionString);
            _s3 = new S3Accessor("cflogget");
            _logParser = new CFLogParser();

            var param = new S3ListParameter
            {
                BucketName = BucketName,
                RootDirectory = RootDirectory,
                StartTime = DateTime.Parse(StartDateTime),
                EndTime = DateTime.Parse(EndDateTime)
            };
            var items = _s3.ListObjects(param).Result;

            var continueDownload = Prompt.GetYesNo($"Target S3 Objects is {items.Count()} count, {items.Sum(i => i.Size):#,#} byte. Continue downloading log files ?", true);
            if (!continueDownload)
                return;

            _tableName = $"{RootDirectory.Replace('-', '_').Replace('/', '_')}_{DateTime.Now:MMdd_HHmm}";
            _database.CreateTable(_tableName);

            Task.WhenAll(items.Select(async (item, idx) =>
            {
                Console.WriteLine($"[START] {idx}:{item.Key}");
                await ExecuteItem(item);
                Console.WriteLine($"[END  ] {idx}:{item.Key}");
            })).Wait();

            var count = _database.Count(_tableName);
            Console.WriteLine($"\n*** Completed to get logs.\nLog count is {count}.\nLogs was stored in '{_tableName}' table.");
        }

        public async Task ExecuteItem(S3Object item)
        {
            IEnumerable<CFLogModel> models;
            try
            {
                var gzPath = $"temp/{item.Key}";
                var logPath = $"temp/{item.Key}".Replace(".gz", ".log");
                await _s3.DropObject(item.BucketName, item.Key, gzPath);
                await _s3.Decompress(gzPath, logPath);

                models = _logParser.Parse(logPath);
                _database.Insert(_tableName, models);

                File.Delete(gzPath);
                File.Delete(logPath);
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Exception occured while {item.Key} processing.", ex);
            }
        }
    }

    public class DateTimeStringAttribute : ValidationAttribute
    {
        public DateTimeStringAttribute()
            : base("The value for {0} must be valid datetime.")
        { }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value != null && !DateTime.TryParse((string)value, out var _))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
