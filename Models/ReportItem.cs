using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMvcApp.Models
{
    public class ReportItem
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string RuleName { get; set; }
        public string RuleScript { get; set; }
        public string RuleID { get; set; } 
    }
}
