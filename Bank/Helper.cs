using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public static class Helper
    {
        // Formats the string to First letter being uppercase, and the rest in lowercase
        public static string FormatString(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
            }
            return input;
        }
        // Prompts the user for a pincode, prints out * and then returns the password as a string
        public static string MaskPincodeData()
        {
            string pin = "";
            ConsoleKeyInfo keyInput;

            do
            {
                keyInput = Console.ReadKey(true);

                if (keyInput.Key != ConsoleKey.Backspace && keyInput.Key != ConsoleKey.Enter)
                {
                    pin += keyInput.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (keyInput.Key == ConsoleKey.Backspace && pin.Length > 0)
                    {
                        pin = pin.Substring(0, (pin.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (keyInput.Key != ConsoleKey.Enter); // Exits the loop when Enter is pressed
            return pin;
        }

        public static bool CheckChange(string input) 
        {
            var inputSplit = input.Split(".");
            if (inputSplit.Length > 1 && inputSplit[1].Length > 2)
            {
                return false;
            }
            return true;
        }

        public static double AccountType()
        {
            Menu AccountTypeMenu = new Menu(new string[] { "Personkonto - 0% Ränta", "Sparkonto - 1.25% Ränta", "Pensionsfond - 4% Ränta" });
            AccountTypeMenu.Output = "Välj typ av konto: ";
            switch (AccountTypeMenu.UseMenu())
            {
                case 0:
                    return 0;
                case 1:
                    return 1.25;
                case 2:
                    return 4;
                default:
                    return 0;
            }
        }

        public static int CurrencyType()
        {
            Menu CurrencyTypeMenu = new Menu(new string[] { "SEK", "USD" });
            CurrencyTypeMenu.Output = "Välj valuta: ";
            switch (CurrencyTypeMenu.UseMenu())
            {
                case 0:
                    return 1; // SEK
                case 1:
                    return 2; // USD
                default:
                    return 1;
            }
        }

        public static void ResetUserData()
        {
            Person.id = -1;
            Person.Email = string.Empty;
            Person.PinCode = string.Empty;
            Person.FirstName = string.Empty;
            Person.LastName = string.Empty;
        }
    }
}
