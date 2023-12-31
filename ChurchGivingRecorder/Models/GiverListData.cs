using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class GiverListData
    {
        public int Id { get; set; }
        public int EnvelopeID { get; set; }
        public string Name { get; set; }
        public bool NeedLetter { get; set; }
        public bool NeedBox { get; set;}
    }
}
