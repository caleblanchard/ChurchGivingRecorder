using System;
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
    }

    public class FundsReportData
    {
        #region Parameters
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        #endregion

        public List<FundTotals> FundTotals { get; set; }
    }
}
