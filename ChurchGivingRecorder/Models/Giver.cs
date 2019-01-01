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
        public int EnvelopeID { get; set; }

        [Display(Name = "Name")]
        [MaxLength(255)]
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
