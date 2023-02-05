using DatabaseTesting;
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
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
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
