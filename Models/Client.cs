using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMvcApp.Models
{
    public class Client
    {
        public string Name { get; set; }
        public string Client_ID { get; set; }
        public List<Rule> Rules { get; set; }
    }
}
