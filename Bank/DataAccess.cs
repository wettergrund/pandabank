using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT * FROM bank_user WHERE id = '{user_id}'", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<BankUserModel> CheckAccess(int user_id)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT * FROM bank_user u INNER JOIN bank_role r ON u.role_id = r.id WHERE u.id = '{user_id}'", new DynamicParameters());
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

        public static int GetUserID(string firstName, string pinCode)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT id FROM bank_user WHERE first_name = '{firstName}' AND pin_code = '{pinCode}'", new DynamicParameters());
                return output.ElementAt(0).id;
            }
        }
        public static void CreateUser(BankUserModel user)
        {
            //ResetIndex();
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Query<BankUserModel>($"INSERT INTO bank_user (first_name, last_name, pin_code, role_id, branch_id) VALUES ('{user.first_name}','{user.last_name}','{user.pin_code}','{user.role_id}','{user.branch_id}')", new DynamicParameters());
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

                if (countPennies.Length > 1 && countPennies[1].Length > 2)
                {
                    Console.WriteLine("Du kan max ange 99 ören");
                    Console.ReadLine();

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

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
