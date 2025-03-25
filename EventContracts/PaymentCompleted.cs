using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public record PaymentCompleted
    {
        public Guid BookingId { get; init; }
        public string BookingNumber { get; init; }
        public decimal Amount { get; init; }
    }
}
