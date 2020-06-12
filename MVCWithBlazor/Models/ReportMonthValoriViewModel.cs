using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class ReportMonthValoriViewModel
    {
        public string Denumire { get; set; }
        public double[,] Valori { get; set; }
        public double[] TotalperZi { get; set; }
        public double TotalperLuna { get; set; }
    }
}
