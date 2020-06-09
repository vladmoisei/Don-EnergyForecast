using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class IndexModel
    {
        [Display(Name = "ID")]
        public int IndexModelID { get; set; }
        [Display(Name = "Clock")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataOra { get; set; }
        [Display(Name = "EDIS status")]
        public int EdisStatus { get; set; }

        [Display(Name = "Energy +A")]
        public double IndexEnergyPlusA { get; set; }
        [Display(Name = "Energy -A")]
        public double IndexEnergyMinusA { get; set; }
        [Display(Name = "Energy +Ri")]
        public double IndexEnergyPlusRi { get; set; }
        [Display(Name = "Energy +Rc")]
        public double IndexEnergyPlusRc { get; set; }
        [Display(Name = "Energy -Ri")]
        public double IndexEnergyMinusRi { get; set; }
        [Display(Name = "Energy -Rc")]
        public double IndexEnergyMinusRc { get; set; }
    }
}
