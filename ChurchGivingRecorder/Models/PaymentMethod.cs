using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChurchGivingRecorder.Models
{
    public enum PaymentMethod
    {
        Check,
        Cash,
        ACH,
        Credit,
        Other
    }
}
