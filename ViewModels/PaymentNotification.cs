using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngOauth.ViewModels
{
    public class PaymentNotification
    {

        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public List<PaymentReference> PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentChannel { get; set; }
        public string LastPaymentAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string Currency { get; set; }
        public string ClientInvoiceRef { get; set; }
        public string AmountPaid { get; set; }
   
    }

    public class PaymentReference
    {
        public DateTime PaymentDate { get; set; }
        public DateTime InsertedAt { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
    }
}