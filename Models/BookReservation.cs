using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class BookReservation
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime ReservedOn { get; set; } = DateTime.Now;

        public bool IsFulfilled { get; set; }
        public bool IsNotified { get; set; }
    }

}
