using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChurchGivingRecorder.Models
{
    public class Giver
    {
        [Required, Key]
        public int Id { get; set; }

        [Display(Name = "Envelope Number")]
        [LookupColumn(Filterable = true)]
        public int EnvelopeID { get; set; }

        public string EnvelopIdString { get; set; }

        [Display(Name = "Name")]
        [MaxLength(255)]
        [LookupColumn(Filterable = true)]
        public string Name { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [NotMapped]
        public string EnvelopeNameDisplay
        {
            get { return $"{EnvelopeID} - {Name}"; }
        }
    }
}
