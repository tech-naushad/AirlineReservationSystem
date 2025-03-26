using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageContracts
{
    public record BookingFailedContract
    {
        public Guid TransactionId { get; set; }
        public string BookingRequest { get; set; }
        public string Reason { get; set; }       
    }
}
