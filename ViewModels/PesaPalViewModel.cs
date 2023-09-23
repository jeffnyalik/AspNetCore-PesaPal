using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AngOauth.ViewModels
{
    public class PesaPalViewModel
    {
        public string id { get; set; }
        public string currency { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
        public string callback_url { get; set; }
        public string redirect_mode { get; set; }
        public string notification_id { get; set; }
        public string branch { get; set; }
        public string email_address { get; set; }
        public BillingAddress billing_address {get;set;}
    }

    public class BillingAddress
    {
        public string email_address { get; set; }
        public string phone_number { get; set; }
        public string country_code { get; set; }
        public string first_name { get; set; }
    }
}