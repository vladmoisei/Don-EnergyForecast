using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class MailModel
    {
        [Display(Name = "From Address")]
        public string FromAdress { get; set; }
        [Display(Name = "To Address")]
        public string ToAddress { get; set; }
        [Display(Name = "Subiect")]
        public string Subjsect { get; set; }
        [Display(Name = "Mesaj")]
        public string Messaege { get; set; }
        [Display(Name = "Folder Fisier")]
        public string FilePathFisierDeTrimis { get; set; }
    }
}
