using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class SmsService : ISmsService
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            // Integrate Africa's Talking / Twilio / Safaricom API here
            Console.WriteLine($"SMS to {phoneNumber}: {message}");
            return Task.CompletedTask;
        }
    }
}
