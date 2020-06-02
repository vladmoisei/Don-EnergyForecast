using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class TagModel
    {
        [Key]
        [Display(Name = "Tag ID")]
        public int TagID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The TagName value cannot exceed 50 characters. ")]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The TagAdress value cannot exceed 50 characters. ")]
        public string Adress { get; set; }

        [Required]
        public int PlcModelID { get; set; }

        [Display(Name = "Plc Name")]
        public virtual PlcModel PlcModel { get; set; }
    }
}
