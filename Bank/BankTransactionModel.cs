namespace Bank
{
    public class BankTransactionModel
    {
        public int from_account_id { get; set; }
        public int to_account_id { get; set; }
        public string from_user { get; set; }
        public string to_user { get; set; }
        public string transaction_name { get; set; }
        public string from_account_name { get; set; }
        public string to_account_name { get; set; }
        public decimal amount { get; set; }
        public string timestamp { get; set; }

        // Checks if account is sender or reciever of a transaction
        // And returns a string with the amount and changes the text color 
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
        // Checks if the transaction is a deposit or withdraw
        // Then only shows the to account or from account
        public void SetTransactionName(int account_id)
        {
            if (from_user != to_user)
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
