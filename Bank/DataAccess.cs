using Dapper;
using Npgsql;
using System.Configuration;
using System.Data;

namespace Bank
{
    public static class DataAccess
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
        public static List<BankTransactionModel> GetTransactions(int account_id)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankTransactionModel>($@"SELECT DISTINCT ON(t.timestamp)
                    t.name as transaction_name,
                    t.amount,
                    t.from_account_id,
                    t.to_account_id,
                    a.name as from_account_name,
                    u.first_name as from_user,
                    c.name as to_account_name,
                    r.first_name as to_user,
                    t.timestamp
                    FROM
                    bank_account a
                    JOIN bank_user u ON u.id = a.user_id
                    JOIN bank_transaction t ON a.id = t.from_account_id OR a.id = t.to_account_id
                    JOIN bank_account c ON c.id = t.to_account_id OR c.id = t.from_account_id
                    JOIN bank_user r ON r.id = c.user_id
                    WHERE
                    a.id = {account_id} OR c.id = {account_id};", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<BankUserModel> GetUserData(int user_id)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT first_name, last_name, email, pin_code, role_id , branch_id FROM bank_user WHERE id = '{user_id}'", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<BankUserModel> GetLockedUsers()
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankUserModel>($"SELECT first_name, last_name, id FROM bank_user WHERE is_locked = true", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void UnlockUser(int unlockID)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Query<BankAccountModel>($"UPDATE bank_user SET is_locked = false WHERE id= '{unlockID}'", new DynamicParameters());
                cnn.Query<BankAccountModel>($"UPDATE bank_user SET attempts_left = 3 WHERE id= '{unlockID}'", new DynamicParameters());
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
        public static decimal CheckTotalBalance(int user_id)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($"SELECT SUM(b.balance) AS balance FROM bank_account b WHERE b.user_id = '{user_id}'");
                return output.First().balance;
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
            newAcc.interest_rate = 0;
            newAcc.currency_id = Helper.CurrencyType();
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
                try
                {
                    var output = cnn.Query<BankAccountModel>($"Select id FROM bank_account WHERE user_id = '{userID}' AND EXISTS (SELECT id FROM bank_account) ORDER BY id ASC", new DynamicParameters());
                    return output.ElementAt(0).id;
                }
                catch (NpgsqlException)
                {
                    return -1; // If user doesn't have any accounts, returns -1
                }
            }
        }
        public static void TransferToUser(BankTransactionModel transaction, string transferName)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($@"
                    UPDATE bank_account SET balance=balance - {transaction.amount} WHERE id={transaction.from_account_id};
                    UPDATE bank_account SET balance=balance + {transaction.amount} WHERE id={transaction.to_account_id};", transaction);
                cnn.Execute($"INSERT INTO bank_transaction (name, amount, from_account_id, to_account_id) VALUES ('{transferName}', @amount ,@from_account_id, @to_account_id);", transaction);
                var transactionOutput = cnn.Query($@"
                    SELECT
                        u.first_name as Från_användare,
                        b.name as Från_kontot,
                        t.amount as Summa,
                        r.first_name as Till_användare,
                        TO_CHAR(t.timestamp, 'HH24:MI:SS') as Tid_på_överföringen
                    FROM
                        bank_account b
                        JOIN bank_transaction t ON b.id = @from_account_id
                        JOIN bank_account c ON c.id = @to_account_id
                        JOIN bank_user u ON u.id = b.user_id
                        JOIN bank_user r ON r.id = c.user_id
                    ORDER BY t.timestamp DESC", transaction);
                Console.Clear();
                foreach (KeyValuePair<string, object> kvp in transactionOutput.ElementAt(0))
                {
                    Console.WriteLine(Helper.FormatString(kvp.Key.Replace('_', ' ')) + ": " + kvp.Value);
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
                List<BankUserModel> output = (List<BankUserModel>)cnn.Query<BankUserModel>($"SELECT * FROM bank_user u INNER JOIN bank_role r ON u.role_id = r.id WHERE u.email = '{Person.Email}'", new DynamicParameters());

                isAdmin = output[0].is_admin;
            }
            return isAdmin;
        }

        public static void CreateUserAcc(BankAccountModel Account)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO bank_account (name, interest_rate, user_id, currency_id, balance ) VALUES (@name, @interest_rate, '{Person.id}',@currency_id, @balance )", Account);
            }
        }
        public static void CreateUserAcc(BankAccountModel Account, int userId)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($"INSERT INTO bank_account (name, user_id, currency_id, interest_rate, balance ) VALUES (@name, '{userId}',@currency_id, @interest_rate ,@balance )", Account);
            }
        }

        public static void UpdateLoanAmount(BankLoanModel loan, int userID)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Query($"INSERT INTO bank_loan (name,interest_rate,user_id,amount) VALUES (@name,@interest_rate,'{userID}', @amount)", loan);
            }
        }
        public static void Deposit(int selectedAcc, decimal amount)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($@"
                UPDATE bank_account SET balance=balance + '{amount}' WHERE id='{selectedAcc}';
                INSERT INTO bank_transaction (name, amount, to_account_id) VALUES ('Insättning', '{amount}' ,'{selectedAcc}');");
            }
        }

        public static void Withdraw(int selectedAcc, decimal amount)
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($@"
                UPDATE bank_account SET balance=balance - '{amount}' WHERE id='{selectedAcc}';
                INSERT INTO bank_transaction (name, amount, from_account_id) VALUES ('Uttag', '{amount}' ,'{selectedAcc}');");
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
            // Ta emot currency_name/ID på pengarna och currency_name/ID på vad som det ska omvandlas till
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BankAccountModel>($@"
                SELECT
                    bank_account.id,
                    bank_account.user_id, 
                    bank_account.name AS account_name, 
                    balance, bank_currency.name AS currency_name, 
                    bank_currency.exchange_rate AS test,
                CAST
                    (balance*bank_currency.exchange_rate AS decimal(10,2)) AS SEK 
                FROM
                    bank_account 
                    JOIN bank_currency ON bank_account.currency_id = bank_currency.id
                WHERE bank_account.user_id={userID};");

                return output.ToList();
            }
        }
        public static void LoginAttempt()
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($@"
                UPDATE bank_user
                SET attempts_left = CASE
                    WHEN attempts_left > 0 THEN attempts_left - 1
                    ELSE 0
                END
                WHERE (email = '{Person.Email}');

                UPDATE bank_user
                SET is_locked = CASE
                    WHEN attempts_left = 0 THEN true
                    else false
                END
                WHERE (email = '{Person.Email}');
                ");
            }
        }

        public static void LoginReset()
        {
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                cnn.Execute($@"
                UPDATE bank_user
                SET attempts_left = 3
                WHERE (email = '{Person.Email}');
                ");
            }
        }

        public static bool IsLocked()
        {
            bool result;
            using (IDbConnection cnn = new NpgsqlConnection(LoadConnectionString()))
            {
                List<BankUserModel> output = (List<BankUserModel>)cnn.Query<BankUserModel>($"SELECT is_locked FROM bank_user WHERE email = '{Person.Email}'", new DynamicParameters());
                try
                {
                    result = Convert.ToBoolean(output[0].is_locked);
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        private static string LoadConnectionString(string id = "Default")
        {
            try
            {
                using (IDbConnection cnn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings[id].ConnectionString))
                {
                    cnn.Open();
                    cnn.Close();
                    return ConfigurationManager.ConnectionStrings[id].ConnectionString;
                };
            }
            catch (Npgsql.PostgresException)
            {
                return ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
            }
            catch (System.NullReferenceException)
            {
                return ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
            }
        }
    }
}
