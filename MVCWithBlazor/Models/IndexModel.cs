using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class IndexModel
    {
        public int IndexModelID { get; set; }

        public DateTime DataOra { get; set; }

        public int EdisStatus { get; set; }

        public double IndexEnergyPlusA { get; set; }

        public double IndexEnergyMinusA { get; set; }

        public double IndexEnergyPlusRi { get; set; }

        public double IndexEnergyPlusRc { get; set; }

        public double IndexEnergyMinusRi { get; set; }

        public double IndexEnergyMinusRc { get; set; }
    }
}
