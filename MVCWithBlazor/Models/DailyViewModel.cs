using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class DailyViewModel
    {
        public List<IndexModel> ListaConsumPerZi { get; set; }
        public List<PrognozaEnergieModel> ListaPrognozaPerZi { get; set; }
        public List<AxisLabelData> ChartData { get; set; }
    }
}
