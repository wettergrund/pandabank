using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankTransactionModel
    {
        public int id { get; set; }
        public int from_account_id { get; set; }
        public int to_account_id { get; set; }
        public string from_user { get; set; }
        public string to_user { get; set; }
        public string transaction_name { get; set; }
        public string from_account_name { get; set; }
        public string to_account_name { get;set; }
        public decimal amount { get; set; }
        public string timestamp { get; set; }
        public string currency { get; set; }

        public string GetSignedAmount(int account_id)
        {
            if (account_id == from_account_id)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return $"-{amount}";
            }
            Console.ForegroundColor = ConsoleColor.Green;
            return amount.ToString();
        }

        public void SetTransactionName(int account_id)
        {
            if(from_user != to_user)
            {
                if (account_id == from_account_id)
                {
                    to_account_name = to_user;
                }
                else
                {
                    from_account_name = from_user;
                }
            }
        }
    }
}
