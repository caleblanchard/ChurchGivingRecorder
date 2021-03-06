﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class Deposit
    {
        [Required, Key]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Deposit Date")]
        public DateTime DepositDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal TotalAmount { get; set; }
    }
}
