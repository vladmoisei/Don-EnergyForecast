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

        public float EnergyPlusA { get; set; }

        public float EnergyMinusA { get; set; }

        public float EnergyPlusRi { get; set; }

        public float EnergyPlusRc { get; set; }

        public float EnergyMinusRi { get; set; }

        public float EnergyMinusRc { get; set; }
    }
}
