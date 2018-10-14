using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    public class S3ListParameter
    {
        public string BucketName { get; set; }
        public string RootDirectory { get; set; }
        /// <summary>
        /// Start datetime to get logs. Truncate by hour.
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// End datetime to get logs. Truncate by hour.
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Log file limit to get.
        /// </summary>
        public int ItemLimit { get; set; } = 5000;
    }
}
