using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class ReportMonthViewModel
    {
        public string Denumire { get; set; }
        public int Zi { get; set; }
        public int Ora { get; set; }
        public double[][] Valori { get; set; }
        public double[] TotalperZi { get; set; }
        public double TotalperLuna { get; set; }
    }
}
