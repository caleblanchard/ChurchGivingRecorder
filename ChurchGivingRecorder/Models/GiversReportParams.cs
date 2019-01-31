using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class GiversReportParams
    {
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Giver")]
        public int GiverId { get; set; }

        public List<Giver> Givers { get; set; }

        [Display(Name = "Group By")]
        public GroupBy GroupBy { get; set; }

        [Display(Name = "Report Format")]
        public ReportFormat ReportFormat { get; set; }
    }
}
