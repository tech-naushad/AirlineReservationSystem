using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public interface IPaymentCompleted
    {
        Guid BookingId { get; init; }
        Guid PaymentId { get; set; }

        //string BookingNumber { get; init; }
        //decimal Amount { get; init; }
    }
}
