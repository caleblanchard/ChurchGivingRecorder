using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChurchGivingRecorder.Models
{
    public class GiftDetail
    {
        [Required, Key]
        public long Id { get; set; }

        public long GiftId { get; set; }

        public Gift Gift { get; set; }

        [Display(Name = "Amount")]
        public double Amount { get; set; }

        public int FundId { get; set; }

        public Fund Fund { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }
    }
}
