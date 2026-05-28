using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public enum InventoryTransactionType
    {
        Added,
        Borrowed,
        Returned,
        Damaged,
        Repaired,
        Missing,
        Recovered,
        Adjusted
    }
}
