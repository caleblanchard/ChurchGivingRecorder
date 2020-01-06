using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class Settings
    {
        [Required, Key]
        public int Id { get; set; }

        [Display(Name = "End Year Letter Template")]
        public byte[] EndYearTemplate { get; set; }

    }
}
