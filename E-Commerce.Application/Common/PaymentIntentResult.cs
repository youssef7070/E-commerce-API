using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public  sealed class PaymentIntentResult
    {


   public PaymentIntentResult(string paymentIntendId , string clientSecret) 
    {

            PaymentIntendId = paymentIntendId;

            ClientSecret = clientSecret;

     }

        public string PaymentIntendId { get; } = default!;

        public string ClientSecret { get; } = default!;


    }
}
