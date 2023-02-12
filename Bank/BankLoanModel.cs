using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankLoanModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public decimal amount { get; set; }

        public decimal sek { get; set; }

        public double interest_rate { get; set; }
    }
}
