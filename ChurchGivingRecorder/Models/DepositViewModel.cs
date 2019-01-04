using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class DepositViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Deposit Date")]
        public DateTime DepositDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<Gift> Gifts { get; set; }

        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public double TotalAmount { get; set; }
    }
}
