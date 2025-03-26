using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageContracts
{
    public record TicketContract
    {
        public Guid BookingId { get; init; }
        public Guid BookingNumber { get; init; }
    }
}
