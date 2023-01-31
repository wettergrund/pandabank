using DatabaseTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    public static class Login
    {
        public static bool LoginChecker()
        {
            bool nameCheck = true;
            bool passwordCheck = true;
            bool showLogin = true;
            while(showLogin)
            {
                while (nameCheck)
                {
                    Console.Clear();
                    Console.Write("Användarnamn: ");
                    nameCheck = CheckUserName(Console.ReadLine());
                }
                while (passwordCheck)
                {
                    Console.Clear();
                    Console.WriteLine("Användarnamn: {0}", Person.UserName);
                    Console.Write("Pinkod: ");
                    passwordCheck = CheckPassword(Console.ReadLine());
                }
                // Checks if the user exists in the database
                if (!DataAccess.CheckUserExists(Person.UserName, Person.PinCode))
                {
                    Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
                    Console.ReadKey();
                    nameCheck = true;
                    passwordCheck = true;
                }
                else
                {
                    Person.id = DataAccess.GetUserID(Person.UserName, Person.PinCode);
                    showLogin = false;
                }
            }
            return true;
        }

        // Checks if username only has allowed characters (Alphabetic letters, numbers and underscore)
        private static bool CheckUserName(string username)
        {
            if(Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                Person.UserName = FormatUserName(username);
                return false;
            }
            Console.WriteLine("Fel användarnamn. Försök igen.");
            return true;
        }

        // Formats the username and sets the first letter in uppercase, rest to lowercase
        private static string FormatUserName(string username)
        {
            return username.Substring(0, 1).ToUpper() + username.Substring(1).ToLower(); ;
        }
        //Checks if password only contains integers
        private static bool CheckPassword(string password)
        {
            bool success = int.TryParse(password, out int result);
            if(success)
            {
                Person.PinCode = password;
                return false;
            }
            Console.WriteLine("Endast nummer är tillåtna. Försök igen.");
            return true;
        }
    }
}
