using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class PaymentContract
    {
        public Guid BookingId { get; init; }
        public decimal Amount { get; init; }
    }
}
