using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class ElemSelectieModel
    {
        public double EnergyPlusA { get; set; }
        public double EnergyPlusRi { get; set; }
        public double EnergyMinusRc { get; set; }
        public double CosFiInductiv { get; set; }
        public double CosFiCapacitiv { get; set; }
        public double RiPlusEnergiiOrare { get; set; }
    }
}
