using DatabaseTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    public class Login
    {
        private static string[] menuArray = new string[] { "Email:", "Pinkod:", "Gå tillbaka" };
        readonly Menu LoginMenu = new Menu(menuArray);
        private string menuItem = "";
        public bool LoginChecker()
        {
            ResetLoginData();
            bool login = true;
            while(login)
            {
                switch (LoginMenu.UseMenu())
                {
                    case 0:
                        ResetLoginData();
                        InputEmail();
                        break;
                    case 1:
                        login = InputPincode();
                        break;
                    case 2:
                        return false;
                } 
            }
            return true;
        }
        private void ResetLoginData()
        {
            Person.Email = "";
            Person.PinCode = "";
            LoginMenu.SetMenuItem("Email:", 0);
            LoginMenu.SelectIndex = 0;
            LoginMenu.PrintSystem();
        }
        private void InputEmail()
        {
            bool isEmail = true;
            while (isEmail)
            {
                LoginMenu.MoveCursorRight();
                isEmail = CheckEmail(Console.ReadLine());
                LoginMenu.SetMenuItem("Email:" + menuItem, 0);
            }
        }

        private bool InputPincode()
        {
            bool pinCheck = true;
            while (pinCheck)
            {
                LoginMenu.MoveCursorRight();
                pinCheck = CheckPincode(Console.ReadLine());
                LoginMenu.MoveCursorBottom();
            }

            if (string.IsNullOrWhiteSpace(Person.Email) || !DataAccess.CheckUserInfo(Person.Email, Person.PinCode))
            {
                Console.WriteLine("Fel användarnamn eller lösenord. Försök igen.");
                Console.ReadKey();
            }
            else
            {
                Person.id = DataAccess.GetUserID(Person.Email, Person.PinCode);
                return false;
            }
            return true;
        }

        //Moves the cursor to the bottom, prints the given error/warning message to the user
        //Then moves the cursor back
        private void Warning(string menuItem, string message)
        {
            LoginMenu.MoveCursorBottom();
            Console.WriteLine(message);
            LoginMenu.SetMenuItem(menuItem, LoginMenu.SelectIndex);
            Console.ReadKey(true);
            LoginMenu.MoveCursorTop();
            LoginMenu.PrintSystem();
        }

        // Checks if email only has allowed characters (Alphabetic letters, numbers and underscore)
        private bool CheckEmail(string email)
        {
            if(Regex.IsMatch(email, @"^[a-zA-Z0-9_@.]+$"))
            {
                Person.Email = Helper.FormatString(email);
                menuItem = Person.Email;
                return false;
            }
            ResetLoginData();
            Warning("Email:", "Fel användarnamn eller lösenord. Försök igen.");
            return true;
        }

        //Checks if password only contains integers
        private bool CheckPincode(string pincode)
        {
            bool success = int.TryParse(pincode, out int result);
            if(success && pincode.Length <= 4)
            {
                Person.PinCode = result.ToString();
                return false;
            }
            Warning("Pinkod:", "Endast nummer är tillåtna. Försök igen.");
            return true;
        }
    }
}
