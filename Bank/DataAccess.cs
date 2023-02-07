﻿using Dapper;
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
                if (output.Count() > 0)
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
        public static void CreateUser(BankUserModel user)
        {
            //ResetIndex();
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Query($"INSERT INTO bank_user (first_name, last_name, pin_code, role_id, branch_id, email) VALUES ('{user.first_name}','{user.last_name}','{user.pin_code}','{user.role_id}','{user.branch_id}','{user.email}')", new DynamicParameters());
            }
            BankAccountModel newAcc = new BankAccountModel();
            newAcc.name = "Personkonto";
            newAcc.balance = 0;

            int userId = GetUserID(user.email, user.pin_code);
            CreateUserAcc(newAcc, userId);
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
                var output = cnn.Query($@"
                    UPDATE bank_account SET balance=balance - '{amount}' WHERE id='{from_account}';
                    UPDATE bank_account SET balance=balance + '{amount}' WHERE id='{to_account}';
                    INSERT INTO bank_transaction (name, amount, from_account_id, to_account_id) VALUES ('Överföring', '{amount}','{from_account}', '{to_account}');");

                var transactionOutput = cnn.Query($@"
                    SELECT
                        u.first_name as Från_användare,
                        b.name as Från_kontot,
                        t.amount as Summa,
                        r.first_name as Till_användare,
                        TO_CHAR(t.timestamp, 'HH24:MI:SS') as Tid_på_överföringen
                    FROM
                        bank_account b
                        JOIN bank_transaction t ON b.id = '{from_account}'
                        JOIN bank_account c ON c.id = '{to_account}'
                        JOIN bank_user u ON u.id = b.user_id
                        JOIN bank_user r ON r.id = c.user_id
                    ORDER BY t.timestamp DESC");
                Console.Clear();
                Console.WriteLine($"Summa: {amount} SEK");
                foreach (KeyValuePair<string, object> kvp in transactionOutput.ElementAt(0))
                {
                    Console.WriteLine(Helper.FormatString(kvp.Key.Replace('_', ' ')) + ": " +  kvp.Value);
                }
                Console.ReadKey();
            }
        }
        public static bool AdminAccess()
        {
            //Return true / false if user is admin
            bool isAdmin;
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                List<BankUserModel> output = (List<BankUserModel>)cnn.Query<BankUserModel>($"SELECT * FROM bank_user u INNER JOIN bank_role r ON u.role_id = r.id WHERE u.id = '{Person.id}'", new DynamicParameters());
                isAdmin = output[0].is_admin;
            }
            return isAdmin;
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
            string fromAccountName;
            decimal toBalance;
            string toAccountName;

            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"SELECT balance, name FROM bank_account WHERE id = '{fromAccountID}' ORDER BY name ASC, balance ASC", new DynamicParameters());
                List<BankAccountModel> tempList = output.ToList();
                fromAccountName = tempList[0].name;
                fromBalance = Convert.ToDecimal(tempList[0].balance);

                output = cnn.Query<BankAccountModel>($"SELECT balance, name FROM bank_account WHERE id = '{toAccountID}' ORDER BY name ASC, balance ASC", new DynamicParameters());
                tempList = output.ToList();
                toAccountName = tempList[0].name;
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

            Console.WriteLine($"{amount}kr har förts över mellan [{fromAccountName}] till [{toAccountName}]");     
            Console.ReadKey();
        }
        public static void CreateUserAcc(BankAccountModel Account)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO bank_account (name, user_id, currency_id, balance ) VALUES (@name, '{Person.id}',1, @balance )", Account);
            }
        }
        public static void CreateUserAcc(BankAccountModel Account, int userId)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO bank_account (name, user_id, currency_id, balance ) VALUES (@name, '{userId}',1, @balance )", Account);
            }
        }

        public static void DeleteUserAcc(int delAccount)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM bank_transaction WHERE from_account_id='{delAccount}' OR to_account_id='{delAccount}'");
                cnn.Execute($"DELETE FROM bank_account WHERE id='{delAccount}'");
            }
        }
        public static List<BankAccountModel> CurrencyExchange(int userID)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>(@$"SELECT  bank_account.id,bank_account.user_id, bank_account.name AS account_name, balance, bank_currency.name AS currency_name, bank_currency.exchange_rate AS test,CAST(balance*bank_currency.exchange_rate AS decimal(10,2)) AS SEK FROM  bank_account JOIN bank_currency ON bank_account.currency_id = bank_currency.id 
WHERE bank_account.user_id={userID};");

                return output.ToList();             
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }
}
