﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public interface IPaymentCompletedEvent
    {
        Guid BookingId { get; }
    }
}
