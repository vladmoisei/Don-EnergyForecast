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
        public int Ora { get; set; }
        [Display(Name = "Value +A")]
        public double ValueEnergyPlusA { get; set; }
        [Display(Name = "Value +Ri")]
        public double ValueEnergyPlusRi { get; set; }
        [Display(Name = "Value -Rc")]
        public double ValueEnergyMinusRc { get; set; }
        [Display(Name = "Val CosFi Inductiv")]
        public double CosFiInductiv { get; set; }
        [Display(Name = "Val CosFi Capacitiv")]
        public double CosFiCapacitiv { get; set; }
        [Display(Name = "Facturare Ri+")]
        public double EnergiiOrareFacturareRiPlus { get; set; }
    }
}
