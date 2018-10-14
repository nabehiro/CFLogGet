using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CFLogGet.Tests
{
    [TestClass]
    public class S3AccessorTests
    {
        private const string ProfileName = "cflogget";
        private const string BucketName = "bucketname";
        private const string RootDirectory = "rootdirectory";

        [Ignore]
        [TestMethod]
        public async Task GetHashToken()
        {
            var param = new S3ListParameter
            {
                BucketName = BucketName,
                RootDirectory = RootDirectory
            };

            var accessor = new S3Accessor(ProfileName);
            var token = await accessor.GetHashToken(param);
            Console.WriteLine(token);
        }

        [Ignore]
        [TestMethod]
        public async Task ListBuckets()
        {
            using (var client = S3ClientFactory.CreateClient(ProfileName))
            {
                var response = await client.ListBucketsAsync();
                Console.WriteLine(response.Buckets.Count);
            }
        }

        [Ignore]
        [TestMethod]
        public async Task List_1Day()
        {
            var param = new S3ListParameter
            {
                BucketName = BucketName,
                RootDirectory = RootDirectory,
                StartTime = new DateTime(2018, 10, 10, 10, 0, 0),
                EndTime = new DateTime(2018, 10, 10, 23, 0, 0),
            };

            var accessor = new S3Accessor(ProfileName);
            var items = await accessor.ListObjects(param);

            TestHelper.WriteConsole(items);
        }

        [Ignore]
        [TestMethod]
        public async Task DropObject()
        {
            var bucketName = BucketName;
            var key = $"{RootDirectory}/E3ML3BHZTNV1SW.2018-10-10-01.20e2f9d0.gz";
            var gzFilePath = $"temp/{key}";

            var accessor = new S3Accessor(ProfileName);
            await accessor.DropObject(bucketName, key, gzFilePath);

            var logFilePath = gzFilePath.Replace(".gz", ".log");
            await accessor.Decompress(gzFilePath, logFilePath);

            var parser = new CFLogParser();
            var models = parser.Parse(logFilePath);

            var tableName = $"table_{DateTime.Now:yyyyMMddHHmmss}";
            var database = new DatabaseManager("Data Source=.\\sqlexpress;Initial Catalog=cflogget;Integrated Security=True;Connection Timeout=10");
            database.CreateTable(tableName);
            database.Insert(tableName, models);

            TestHelper.WriteConsole(models);
        }
    }
}
