using DatabaseTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    public class Login
    {
        readonly Menu LoginMenu = new Menu(new string[] { "Email:", "Pinkod:", "Gå tillbaka" });
        public bool LoginChecker()
        {
            ResetLoginData();
            bool login = true;
            while (login)
            {
                switch (LoginMenu.UseMenu())
                {
                    case 0:
                        ResetLoginData();
                        GetUserEmail();
                        break;
                    case 1:
                        login = GetUserPincode();
                        break;
                    case 2:
                        return false;
                }
            }
            return true;
        }
        // Prompts the user to enter their email, then validates it
        private void GetUserEmail()
        {
            bool isEmail = true;
            while (isEmail)
            {
                LoginMenu.MoveCursorRight();
                isEmail = ValidateEmail(Console.ReadLine());
            }
        }
        // Prompts the user to enter their pincode and checks if its valid
        private bool GetUserPincode()
        {
            bool pinCheck = true;
            while (pinCheck)
            {
                LoginMenu.MoveCursorRight();
                //Checks if password only contains integers and masks the pincode output in the console
                pinCheck = ValidatePincode(Helper.MaskPincodeData());
                ResetRow("Pinkod:");
                LoginMenu.MoveCursorBottom();
            }
            //If email is empty or Email/Pincode combo is wrong, gives the user a warning
            //If they are correct, gets the ID and allows the user to log in
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

        //Resets the stored email/pincode for Person.cs
        //Also resets the menu prints to remove the previously stored email
        private void ResetLoginData()
        {
            Person.Email = "";
            Person.PinCode = "";
            LoginMenu.SetMenuItem("Email:", 0);
            LoginMenu.SelectIndex = 0;
            LoginMenu.PrintSystem();
        }

        private void ResetRow(string row)
        {
            LoginMenu.SetMenuItem(row, LoginMenu.SelectIndex);
            LoginMenu.PrintSystem();
        }

        //Moves the cursor to the bottom, prints the given error/warning message to the user
        //Resets the menu output and moves the cursor back
        private void Warning(string menuItem, string errorMessage)
        {
            LoginMenu.MoveCursorBottom();
            Console.WriteLine(errorMessage);
            LoginMenu.SetMenuItem(menuItem, LoginMenu.SelectIndex);
            Console.ReadKey(true);
            LoginMenu.MoveCursorTop();
        }

        // Checks if email only has allowed characters (Alphabetic letters, numbers and underscore)
        private bool ValidateEmail(string email)
        {
            if (Regex.IsMatch(email, @"^[a-zA-Z0-9_@.]+$"))
            {
                Person.Email = Helper.FormatString(email);
                LoginMenu.SetMenuItem("Email:" + Person.Email, 0);
                return false;
            }
            ResetLoginData();
            Warning("Email:", "Fel användarnamn eller lösenord. Försök igen.");
            return true;
        }

        //Checks if password only contains integers
        private bool ValidatePincode(string pincode)
        {
            bool success = int.TryParse(pincode, out int result);
            if (success && pincode.Length <= 4)
            {
                Person.PinCode = result.ToString();
                return false;
            }
            Warning("Pinkod:", "Endast nummer är tillåtna. Försök igen.");
            return true;
        }
    }
}