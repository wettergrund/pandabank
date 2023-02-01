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
            List<BankUserModel> getPermission = DataAccess.CheckAccess(Person.id);
            bool isAdmin = getPermission[0].is_admin;

            var loggedOnUser = DataAccess.GetUserData(Person.id).FirstOrDefault();

            Console.WriteLine($"{loggedOnUser.full_name}");
            Console.ReadLine();
            

            bool showMenu = true;
            while (showMenu)
            {
                Menu UserMenu;

                if (isAdmin)
                {
                    UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton", "Logga ut", " ","ADMIN" });
                }
                else
                {
                    UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton", "Logga ut" });

                }
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
                    case 4:
                        AdminMenu();
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

        static void AdminMenu()
        {
            Menu CreateAcoountMenu = new Menu(new string[] { "Skapa användare", "Gå tillbaka" });
            bool showMenu = true;
            while (showMenu) { 

                switch (CreateAcoountMenu.UseMenu())
                {
                    case 0:
                        BankUserModel newUser = new BankUserModel();

                       
                        Console.Write("Ange förnamn: ");
                        newUser.first_name = Console.ReadLine();
                        Console.Write("Ange efternamn: ");
                        newUser.last_name = Console.ReadLine();

                        Random random = new Random();
                        newUser.pin_code = Convert.ToString(random.Next(1000, 9999));

                        Console.WriteLine($"Skapa {newUser.full_name} med pinkod {newUser.pin_code}");
                        Console.ReadLine();
                        DataAccess.CreateUser(newUser);


                        break;
                    case 1:
                        Console.WriteLine("Gå tillbaka");
                        showMenu = false;
                        break;
                    case 2:
                        break;
                }
            }

        }
    }
}