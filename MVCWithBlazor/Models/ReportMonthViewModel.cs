using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class ReportMonthViewModel
    {
        public int Luna { get; set; }
        public int An { get; set; }
        public int ZileInLuna { get; set; }
        public ReportMonthValoriViewModel[] TabeleValori { get; set; }
    }
}
