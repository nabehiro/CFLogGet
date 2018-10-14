using Amazon;
using Amazon.Runtime.CredentialManagement;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    [Command("register" , Description = "Register AWS credential profile for downloading CloudFront logs from s3. The profile is added to the SDK Store, the profile'name is 'cflogget'. For more information, see https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html.")]
    public class RegisterProfileCommand : CommandBase
    {
        [Required]
        [Option(Description = "Required. Access key")]
        public string AccessKey { get; set; }

        [Required]
        [Option(Description = "Required. Secret key")]
        public string SecretKey { get; set; }

        [Required]
        [RegionEndpoint]
        [Option(Description = "Required. Region(e.g. us-east-2). For more infomation, see https://docs.aws.amazon.com/general/latest/gr/rande.html.")]
        public string Region { get; set; }

        private const string ProfileName = "cflogget";

        public override void Execute()
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = AccessKey,
                SecretKey = SecretKey
            };

            var profile = new CredentialProfile(ProfileName, options)
            {
                Region = RegionEndpoint.GetBySystemName(Region)
            };

            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);

            Console.WriteLine($"*** Completed registration for AWS credential profile as named '{ProfileName}' on SDK Store.");
        }
    }

    public class RegionEndpointAttribute : ValidationAttribute
    {
        public RegionEndpointAttribute()
            : base("The value for {0} must be valid region endpoint(e.g. us-east-2).")
        { }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value != null && !RegionEndpoint.EnumerableAllRegions.Any(r => r.SystemName == (string)value))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
