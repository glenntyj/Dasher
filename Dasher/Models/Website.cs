using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dasher.Models
{
    public class Website
    {
        public int Id { get; set; }

        public string Company { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public DateTime LastScraped { get; set; }
    }
}
