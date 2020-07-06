using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class PrognozaEnergieModel
    {
        public int PrognozaEnergieModelID { get; set; }
        [Display(Name = "Clock")]

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataOra { get; set; }
        public int Ora { get; set; }
        public float Valoare { get; set; }

    }
}
