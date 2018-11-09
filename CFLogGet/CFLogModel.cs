using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLogGet
{
    public class CFLogModel
    {
        // local datetime (only getter)
        [Column(TypeName = "datetime2(0)")]
        public DateTime dt => DateTime.Parse($"{date} {time}", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

        // if StringLength attribute is specified, truncate string by maximum length.
        [StringLength(10)]
        [Column(Order = 0, TypeName = "nvarchar(10)")]
        public string date { get; set; }
        [StringLength(10)]
        [Column(Order = 1, TypeName = "nvarchar(10)")]
        public string time { get; set; }
        [StringLength(20)]
        [Column(Order = 2, TypeName = "nvarchar(20)")]
        public string x_edge_location { get; set; }
        [Column(Order = 3, TypeName = "int")]
        public int? sc_bytes { get; set; }
        [StringLength(20)]
        [Column(Order = 4, TypeName = "nvarchar(20)")]
        public string c_ip { get; set; }
        [StringLength(10)]
        [Column(Order = 5, TypeName = "nvarchar(10)")]
        public string cs_method { get; set; }
        [StringLength(50)]
        [Column(Order = 6, TypeName = "nvarchar(50)")]
        public string cs_host { get; set; }
        [StringLength(450)]
        [Column(Order = 7, TypeName = "nvarchar(450)")]
        public string cs_uri_stem { get; set; }
        [Column(Order = 8, TypeName = "int")]
        public int? sc_status { get; set; }
        [Column(Order = 9, TypeName = "nvarchar(max)")]
        public string cs_referer { get; set; }
        [Column(Order = 10, TypeName = "nvarchar(max)")]
        public string cs_user_agent { get; set; }
        [Column(Order = 11, TypeName = "nvarchar(max)")]
        public string cs_uri_query { get; set; }
        [Column(Order = 12, TypeName = "nvarchar(max)")]
        public string cs_cookie { get; set; }
        [StringLength(10)]
        [Column(Order = 13, TypeName = "nvarchar(10)")]
        public string x_edge_result_type { get; set; }
        [StringLength(100)]
        [Column(Order = 14, TypeName = "nvarchar(100)")]
        public string x_edge_request_id { get; set; }
        [StringLength(50)]
        [Column(Order = 15, TypeName = "nvarchar(50)")]
        public string x_host_header { get; set; }
        [StringLength(5)]
        [Column(Order = 16, TypeName = "nvarchar(5)")]
        public string cs_protocol { get; set; }
        [Column(Order = 17, TypeName = "int")]
        public int? cs_bytes { get; set; }
        [Column(Order = 18, TypeName = "decimal(10, 3)")]
        public decimal? time_taken { get; set; }
        [StringLength(100)]
        [Column(Order = 19, TypeName = "nvarchar(100)")]
        public string x_forwarded_for { get; set; }
        [StringLength(20)]
        [Column(Order = 20, TypeName = "nvarchar(20)")]
        public string ssl_protocol { get; set; }
        [StringLength(100)]
        [Column(Order = 21, TypeName = "nvarchar(100)")]
        public string ssl_cipher { get; set; }
        [StringLength(10)]
        [Column(Order = 22, TypeName = "nvarchar(10)")]
        public string x_edge_response_result_type { get; set; }
        [StringLength(10)]
        [Column(Order = 23, TypeName = "nvarchar(10)")]
        public string cs_protocol_version { get; set; }
        [Column(Order = 24, TypeName = "nvarchar(max)")]
        public string fle_status { get; set; }
        [Column(Order = 25, TypeName = "nvarchar(max)")]
        public string fle_encrypted_fields { get; set; }
    }
}
