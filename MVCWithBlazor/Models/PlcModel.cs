using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class PlcModel
    {
        [Display(Name = "Plc ID")]
        public int PlcModelID { get; set; }
        [Required(ErrorMessage = "Numele este obligatoriu! ")]
        [StringLength(50, ErrorMessage = "The PlcName value cannot exceed 50 characters. ")]
        public string Name { get; set; }

        [Display(Name = "Release Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataCreation { get; set; }

        [Required]
        public bool IsEnable { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The PlcIp value cannot exceed 15 characters. ")]
        public string Ip { get; set; }

        [Required]
        public short Rack { get; set; }

        [Required]
        public short Slot { get; set; }

        [NotMapped]
        public virtual int PingRequestsFail { get; set; }
        public virtual List<TagModel> TagsList { get; set; }
    }
}
