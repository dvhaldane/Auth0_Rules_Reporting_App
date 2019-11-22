using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMvcApp.Models
{
    public class Rule
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool Enabled { get; set; }
        public string Script { get; set; }
        public int Order { get; set; }
        public string Stage { get; set; }
    }
}
