using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public interface ITicketProcessed
    {
        Guid BookingId { get; init; }
        string TicketNumber { get; }
    }
}
