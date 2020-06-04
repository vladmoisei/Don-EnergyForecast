using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class ConsumEnergieModel
    {
        public int ConsumEnergieModelID { get; set; }
        public DateTime Data { get; set; }
        public int Ora { get; set; }
        public float Valoare { get; set; }
    }
}
