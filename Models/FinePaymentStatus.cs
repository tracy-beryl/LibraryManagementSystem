using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public enum FinePaymentStatus
    {
        None,
        Unpaid,
        Pending,
        Paid,
        Reversed
    }
}
