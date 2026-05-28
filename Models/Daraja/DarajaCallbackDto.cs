using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models.Daraja
{
    public class DarajaCallbackDto
    {
        public CallbackBody Body { get; set; }
    }

    public class CallbackBody
    {
        public StkCallback stkCallback { get; set; }
    }

    public class StkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public CallbackMetadata CallbackMetadata { get; set; }
    }

    public class CallbackMetadata
    {
        public Item[] Item { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
