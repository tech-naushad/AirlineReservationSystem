using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public record PaymentFailed
    {
        public Guid BookingId { get; init; }
        public string BookingNumber { get; init; }
        public string Reason { get; init; }
    }
}
