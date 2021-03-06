﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class FundTotals
    {
        public string FundName { get; set; }

        public Dictionary<string, decimal> Data { get; set; }

        public decimal Total { get; set; }
    }

    public class FundsReportData
    {
        #region Parameters
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime EndDate { get; set; }
        #endregion

        public List<FundTotals> FundTotals { get; set; }

        public decimal Total { get; set; }
    }
}
