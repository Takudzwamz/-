using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faculty.Models
{
    public class PerformanceViewModel
    {
        public string StudentName { get; set; }
        public List<PerformanceListItem> ListLines { get; set; }
    }
    public class PerformanceListItem
    {
        public string Subject { get; set; }
        public string Mark { get; set; }
    }
}
