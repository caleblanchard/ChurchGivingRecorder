using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{

    public class FundData
    {
        public int FundId { get; set; }

        public decimal Total { get; set; }
    }

    public class GiverTotals
    {
        public string GiverName { get; set; }

        public Dictionary<string, Dictionary<int, decimal>> Data { get; set; }

        public Dictionary<int, string> Funds { get; set; }

        public decimal Total { get; set; }
    }

    public class GiversReportData
    {
        #region Parameters
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime EndDate { get; set; }
        #endregion

        public List<GiverTotals> GiverTotals { get; set; }

        public decimal Total { get; set; }
    }
}
