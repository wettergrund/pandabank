using DatabaseTesting;
using System.Globalization;

namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            ShowMenu();
        }
        // Shows a main menu to the user with options to login, or to exit the program
        private static void ShowMenu()
        {
            Menu MainMenu = new Menu(new string[] { "Logga in", "Stäng programmet" });
            bool showMenu = true;
            while(showMenu)
            {
                switch (MainMenu.UseMenu())
                {
                    case 0:
                        if(Login())
                        {
                            ShowUserMenu();
                        }
                        break;
                    case 1:
                        showMenu = false;
                        break;
                }
            }
        }
        // If user logs in succesfully - This will show different options that the user has
        // Checking balance to their accounts, transfers between their own accounts and logging out.
        private static void ShowUserMenu()
        {
            Menu UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton", "Logga ut" });
            bool showMenu = true;
            while (showMenu)
            {
                switch (UserMenu.UseMenu())
                {
                    case 0:
                        ShowAccountBalance(Person.id);
                        break;
                    case 1:
                        MoveMoney(Person.id);
                        break;
                    case 2:
                        showMenu = false;
                        break;
                }
            }
        }
        // A basic login prototype with no error handling :D
        private static bool Login()
        {
            Console.Write("Förnamn: ");
            Person.FirstName = Console.ReadLine();
            Console.Write("Lösenord: ");
            Person.PinCode = Console.ReadLine();
            Person.id = DataAccess.GetUserID(Person.FirstName, Person.PinCode);

            if (Person.id > 0)
            {
                Console.WriteLine("Du har loggats in.");
                return true;
            }
            else
            {
                return false;
            }
        }
        // Calls the method to create a menu to show account balances
        public static void ShowAccountBalance(int userID)
        {
            Menu BalanceMenu = new Menu();
            BalanceMenu.CreateTransferMenu(userID, -1);
            BalanceMenu.UseMenu();
        }
        // Method that allows user to send money between their own accounts
        private static void MoveMoney(int userId = 2)
        {
            int selectedFromId = SelectAccount.FromID();
            if(selectedFromId > 0)
            {
                int selectedToId = SelectAccount.ToID(selectedFromId);
                if(selectedToId> 0)
                {
                    DataAccess.UpdateBalance(selectedFromId, selectedToId);
                }
            }
        }
    }
}