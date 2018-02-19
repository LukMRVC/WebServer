using System;
using Braintree;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Http
{
    class BraintreeClient
    {

        //Nastavení Braintree
        public static BraintreeGateway gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            MerchantId = "ghv94zkc2x36bxwc",
            PublicKey = "833vs2h75r24h67m",
            PrivateKey = "704e435eb9199ea6f9210b786125c445",
        };

        public class Details
        {
            public int LastTwo { get; set; }
            public int LastFour { get; set; }
            public string CardType { get; set; }
        }

        public class BinData
        {
            public string Prepaid { get; set; }
            public string Healthcare { get; set; }
            public string Debit { get; set; }
            public string DerbinRegulated { get; set; }
            public string Commercial { get; set; }
            public string Payroll { get; set; }
            public string IssuingBank { get; set; }
            public string CountryOfIssuance { get; set; }
            public string ProductId { get; set; }
        }

        public class PaymentRequest
        {
            public string Nonce { get; set; }
            public Details Details { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public BinData BinData { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }

    }
}
