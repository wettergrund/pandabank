using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankAccountModel
    {
        public BankAccountModel()
        {
            currency_id = 1;
        }
        public int id { get; set; }

        public string name { get; set; }

        public decimal balance { get; set; }

        public decimal sek { get; set; }

        public double interest_rate { get; set; }

        public string currency_name { get; set; }

        public int currency_id { get; set; }

        public double currency_exchange_rate { get; set; }
    }
}
