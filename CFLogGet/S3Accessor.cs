using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFLogGet
{
    public class S3Accessor
    {
        private readonly string _profileName;

        public S3Accessor(string profileName)
        {
            _profileName = profileName;
        }

        public AmazonS3Client CreateClient() => S3ClientFactory.CreateClient(_profileName);

        public async Task<IEnumerable<S3Object>> ListObjects(S3ListParameter param)
        {
            var startTimeUtc = param.StartTime.ToUniversalTime();
            var endTimeUtc = param.EndTime.ToUniversalTime();
            var hashToken = await GetHashToken(param);

            var items = new List<S3Object>();

            var startTimeKey = $"{param.RootDirectory}/{hashToken}.{startTimeUtc:yyyy-MM-dd-HH}.00000000";
            var endTimeKey = $"{param.RootDirectory}/{hashToken}.{endTimeUtc:yyyy-MM-dd-HH}.99999999";
            var prefixDate = startTimeUtc.Date;
            while (prefixDate < endTimeUtc)
            {
                // Query condition is only prefix, so retrive one day log files at a time and filter.
                var subItems = await ListObjects(param.BucketName,
                    $"{param.RootDirectory}/{hashToken}.{prefixDate:yyyy-MM-dd}");

                items.AddRange(subItems.Where(i =>
                    i.Key.CompareTo(startTimeKey) >= 0 && i.Key.CompareTo(endTimeKey) <= 0));

                prefixDate = prefixDate.AddDays(1);
            }

            return items;
        }

        public async Task<IEnumerable<S3Object>> ListObjects(string bucketName, string prefix)
        {
            var items = new List<S3Object>();
            using (var client = CreateClient())
            {
                var request = new ListObjectsV2Request { BucketName = bucketName, Prefix = prefix };

                ListObjectsV2Response response;
                do
                {
                    response = await client.ListObjectsV2Async(request);
                    items.AddRange(response.S3Objects);
                } while (response.IsTruncated);
            }

            return items;
        }

        public async Task DropObject(string bucketName, string key, string filePath)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (var client = CreateClient())
            {
                using (var response = await client.GetObjectAsync(request))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    CancellationToken cancellationToken;
                    await response.WriteResponseStreamToFileAsync(filePath, false, cancellationToken);
                }
            }
        }

        public async Task Decompress(string srcPath, string dstPath)
        {
            using (FileStream gzFileStream = File.OpenRead(srcPath))
            using (FileStream logFileStream = File.Create(dstPath))
            using (GZipStream gzipStream = new GZipStream(gzFileStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(logFileStream);
            }
        }

        /// <summary>
        /// Get {HASH_TOKEN} string.
        /// https://s3-ap-northeast-1.amazonaws.com/{BUCKET_NAME}/{ROOT_DIRECTORY}/{HASH_TOKEN}.YYYY-mm-DD-HH.9281d331.gz
        /// 
        /// HASH_TOKEN is determinted by the value of BUCKET_NAME and ROOT_DIRECTORY.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> GetHashToken(S3ListParameter param)
        {
            using (var client = CreateClient())
            {
                var prefix = param.RootDirectory + "/";

                var request = new ListObjectsV2Request
                {
                    BucketName = param.BucketName,
                    Prefix = prefix,
                    MaxKeys = 1
                };

                var response = await client.ListObjectsV2Async(request);
                var key = response?.S3Objects.FirstOrDefault()?.Key;
                if (key == null)
                    return null;

                key = key.Substring(prefix.Length);
                return key.Substring(0, key.IndexOf('.'));
            }
        }
    }
}
