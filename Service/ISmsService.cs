using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
   
        public interface ISmsService
        {
            Task SendAsync(string phoneNumber, string message);
        }

    
}
