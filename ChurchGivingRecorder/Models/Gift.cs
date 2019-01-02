﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChurchGivingRecorder.Models
{
    public class Gift
    {
        [Required, Key]
        public long Id { get; set; }

        public int DepositId { get; set; }

        public Deposit Deposit { get; set; }

        [Display(Name = "Giver")]
        public int GiverId { get; set; }

        public Giver Giver { get; set; }

        [Display(Name = "Gift Date")]
        [DataType(DataType.Date)]
        public DateTime GiftDate { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Check Number")]
        public string CheckNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual List<GiftDetail> GiftDetails { get; set; }
    }
}
