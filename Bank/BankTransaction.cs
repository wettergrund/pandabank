using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankTransaction
    {
        public int id { get; set; }

        public string name { get; set; }

        public string from_account_name { get; set; }

        public string to_account_name { get;set; }
        public decimal amount { get; set; }

        public string timestamp { get; set; } 
        
    }
}
