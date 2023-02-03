using Dapper;
using DatabaseTesting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class DataAccess
    {
        // Gets account data for the given user_id
        public static List<BankAccountModel> GetAccountData(int user_id)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"SELECT name, balance, id FROM bank_account WHERE user_id = '{user_id}' ORDER BY name ASC, balance ASC, id ASC", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<BankUserModel> GetUserData(int user_id)
        {
            using(IDbConnection cnn =new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT first_name, last_name, pin_code , role_id , branch_id FROM bank_user WHERE id = '{user_id}'", new DynamicParameters());
                return output.ToList();

            }
        }
        public static List<BankAccountModel> GetTransferAccountData(int user_id, int accountID)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"SELECT name, balance, id FROM bank_account WHERE user_id = '{user_id}' AND id != '{accountID}' ORDER BY name ASC, balance ASC, id ASC", new DynamicParameters());
                return output.ToList();
            }
        }
        // Checks if there are enough funds in the account
        public static bool CheckAccountFunds(int accountID, decimal funds)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query($"SELECT balance FROM bank_account WHERE id = '{accountID}' AND balance >= '{funds}'");
                if(output.Count() > 0)
                {
                    return true;
                }
                return false;
            }
        }
        // Checks if the users exists in the database
        public static bool CheckUserExists(string email)
        {
            using (NpgsqlConnection cnn = new NpgsqlConnection(LoadConnectionString())) 
            {
                cnn.Open();
                var sql = $"SELECT COUNT(*) FROM bank_user WHERE email = '{email}'";
                var cmd = new NpgsqlCommand(sql, cnn);
                cmd.Parameters.AddWithValue("email", email);
                bool userExists = (long)cmd.ExecuteScalar() > 0;
                if(userExists)
                {
                    cnn.Close();
                    return true;
                }
                else
                {
                    cnn.Close();
                    return false;
                }
            }
        }
        //Checks if the user + pincode combo is correct
        public static bool CheckUserInfo(string email, string pin)
        {
            using (NpgsqlConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Open();
                var sql = $"SELECT COUNT(*) FROM bank_user WHERE email = '{email}' AND pin_code = '{pin}'";
                var cmd = new NpgsqlCommand(sql, cnn);
                cmd.Parameters.AddWithValue("email", email);
                bool userExists = (long)cmd.ExecuteScalar() > 0;
                if (userExists)
                {
                    cnn.Close();
                    return true;
                }
                else
                {
                    cnn.Close();
                    return false;
                }
            }
        }

        public static int GetUserID(string email)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT id FROM bank_user WHERE email = '{email}'", new DynamicParameters());
                return output.ElementAt(0).id;
            }
        }

        public static int GetUserID(string email, string pinCode)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT id FROM bank_user WHERE email = '{email}' AND pin_code = '{pinCode}'", new DynamicParameters());
                return output.ElementAt(0).id;
            }
        }

        public static int GetAccountID(int userID)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"Select id FROM bank_account WHERE user_id = '{userID}' ORDER BY id ASC", new DynamicParameters());
                return output.ElementAt(0).id;
            }
        }

        public static void TransferToUser(int from_account, int to_account, decimal amount)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Execute($@"
                    UPDATE bank_account SET balance=balance - '{amount}' WHERE id='{from_account}';
                    UPDATE bank_account SET balance=balance + '{amount}' WHERE id='{to_account}';
                    INSERT INTO bank_transaction (name, from_account_id, to_account_id) VALUES ('Överföring - {amount}', '{from_account}', '{to_account}');");
            }
        }

        public static void UpdateBalance(int fromAccountID, int toAccountID)
        {
            /* Method that move money from one account to another account, based on accounts DB ID */
            decimal amount;
            bool successfulInput;

            do {
                Console.Clear();
                Console.WriteLine("Ange en summa");
                string? enteredValue = Console.ReadLine();

                enteredValue = enteredValue.Replace(",", ".");
                var countPennies = enteredValue.Split('.');


                successfulInput = decimal.TryParse(enteredValue, out amount);

                if(amount < 0)
                {
                    Console.WriteLine("Negativa summor funkar ej.");
                    Console.ReadKey();
                    successfulInput = false;
                }
                if (countPennies.Length > 1 && countPennies[1].Length > 2)
                {
                    Console.WriteLine("Du kan max ange 99 ören");
                    Console.ReadKey();

                    successfulInput = false;
                }
            }
            while (!successfulInput);


            decimal fromBalance;
            decimal toBalance;

            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"SELECT balance FROM bank_account WHERE id = '{fromAccountID}' ORDER BY name ASC, balance ASC", new DynamicParameters());
                List<BankAccountModel> tempList = output.ToList();
                fromBalance = Convert.ToDecimal(tempList[0].balance);

                output = cnn.Query<BankAccountModel>($"SELECT balance FROM bank_account WHERE id = '{toAccountID}' ORDER BY name ASC, balance ASC", new DynamicParameters());
                tempList = output.ToList();
                toBalance = Convert.ToDecimal(tempList[0].balance);
            }

            fromBalance -= amount;
            toBalance += amount;

            if (fromBalance < 0)
            {
                Console.WriteLine("Inte tillräckligt med pengar");
                Console.ReadKey();
                return;
            }

            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Query<BankAccountModel>($"UPDATE bank_account SET balance = '{fromBalance}' WHERE id= '{fromAccountID}'", new DynamicParameters());
                cnn.Query<BankAccountModel>($"UPDATE bank_account SET balance = '{toBalance}' WHERE id='{toAccountID}'", new DynamicParameters());
            }

            Console.WriteLine($"Nytt saldo: (från) {fromBalance} och  (till) {toBalance}");
            Console.ReadKey();
        }
        public static void CreateUserAcc(BankAccountModel Account)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO bank_account (name, user_id, currency_id, balance ) VALUES (@name, '{Person.id}',1, @balance )", Account);
            }
        }

        public static void DeleteUserAcc(int delAccount)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM bank_account WHERE id='{delAccount}'");
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }
}
