using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    public static class S3ClientFactory
    {
        public static AmazonS3Client CreateClient(string profileName)
        {
            const string errorMessage = "Failed to load AWS credential profile, '{0}'({1}). If not registered profile, run cflogget.exe register ... ";

            var chain = new CredentialProfileStoreChain();
            if (!chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
            {
                throw new ApplicationException(string.Format(errorMessage, profileName, "TryGetAWSCredentials"));
            }
            if (!chain.TryGetProfile(profileName, out CredentialProfile profile))
            {
                throw new ApplicationException(string.Format(errorMessage, profileName, "TryGetProfile"));
            }

            return new AmazonS3Client(awsCredentials, profile.Region);
        }
    }
}
