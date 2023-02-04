using DatabaseTesting;
using System.Globalization;
using System.Text.RegularExpressions;

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
            Login BankLogin = new Login(); // 
            bool showMenu = true;
            while(showMenu)
            {
                if (Person.attempts == 3)
                {
                    showMenu = false;
                    break;
                }

                switch (MainMenu.UseMenu())
                {
                    case 0:
                        
                        if (BankLogin.LoginChecker())
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
            DataAccess.LoginReset();
            Menu UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton","Skapa ett nytt konto","Ta bort ett konto", "Logga ut" });
            
            bool isAdmin = DataAccess.AdminAccess();
            bool showMenu = true;

            while (showMenu)
            {
                switch (UserMenu.UseMenu())
                {
                    case 0:
                        ShowAccountBalance(Person.id);
                        break;
                    case 1:
                        MoveMoney();
                        break;
                    case 2:
                        CreatNewAcc();
                        break;
                    case 3:
                        DeleteAcc(Person.id);
                        break;
                    case 4:
                        showMenu = false;
                        break;
                }
            }
        }

        // Calls the method to create a menu to show account balances
        public static void ShowAccountBalance(int userID)
        {
            Menu BalanceMenu = new Menu();
            BalanceMenu.CreateTransferMenu(userID);
            BalanceMenu.UseMenu();
        }
        // Method that allows user to send money between their own accounts
        private static void MoveMoney()
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
        //Method to creat a savings account
        public static void CreatNewAcc()
        {
                BankAccountModel newAcc = new BankAccountModel();
                Console.Write("Kontonamn: ");
                string accName = Console.ReadLine();
                if (!Regex.IsMatch(accName, @"^[a-zA-Z]+$"))
                {
                    Console.WriteLine("Du kan bara skapa konton med bokstäver");
                    Console.ReadKey();
                }
                else
                {
                    newAcc.name = accName;
               bool success = true;
                do
                {
                    Console.WriteLine("Hur mycket pengar vill du sätta in?");
                    Console.Write(accName + ": ");

                    string? amount = Console.ReadLine();
                    amount = amount.Replace(",", ".");
                    var amountsplit = amount.Split(".");
                    bool succAmount = decimal.TryParse(amount, out decimal accValue);

                    if (amountsplit.Length > 1 && amountsplit[1].Length > 2)
                    {
                        Console.WriteLine("Du kan max ange 99 ören");
                        Console.ReadKey();
                        success = false;
                    }
                    else if(!succAmount)
                    {
                    Console.WriteLine("Var vänligen använde inte bokstäver");
                    Console.ReadKey();
                    }
                    else
                    {
                        if(accValue < 0)
                        {
                            Console.WriteLine("Du kan inte sätt in minus duh");
                            Console.ReadKey();
                        }
                        else
                        {
                    newAcc.balance= accValue;
                    Console.WriteLine(newAcc.name + newAcc.balance);
                    Console.ReadKey();
                    DataAccess.CreateUserAcc(newAcc);
                    success= true;

                        }
                    }
                }
                while (!success);
                }
        }

        public static void DeleteAcc(int userID)
        {
            
            Menu accountMenu = new Menu();
            List<BankAccountModel> selectedAcc = accountMenu.CreateTransferMenu(userID, -1);
            List<BankUserModel> pincheck = DataAccess.GetUserData(userID);
            int deleteAcc;
            while(true)
            {
                accountMenu.Output = "Välj konto att radera.";
                deleteAcc = accountMenu.UseMenu();
                if (deleteAcc == accountMenu.GetMenu().Length - 1)
                {
                    break;
                }
                else if (selectedAcc[deleteAcc].balance > 0)
                {
                    Console.WriteLine("Du har pengar kvar på detta konto, sicka över pengarna till ett annat för att radera.");
                    Console.ReadKey();
                    break;
                }
                else
                {
                    Console.WriteLine("Vänligen skriv in din pinkod för ta bort kontot.");
                    Console.Write("Pinkod: ");
                    string pincode = Console.ReadLine();
                    if (pincode == pincheck.First().pin_code)
                    { 
                        Console.WriteLine("Ditt konto " + selectedAcc[deleteAcc].name + " har raderats.");
                        Console.ReadKey();
                        DataAccess.DeleteUserAcc(selectedAcc[deleteAcc].id);
                    break;
                    }
                    else
                    {
                        Console.WriteLine("Fel pinkod skriv rätt hallå");
                        Console.ReadKey();
                        
                    }
                }
            }

        }
        //Method to convert currency
        public static void CurrencyConvert()
        {
            List<BankAccountModel> bankAccountModels = DataAccess.CurrencyExchange(1);

            foreach (var bankAccountModel in bankAccountModels)
            {

                Console.WriteLine(bankAccountModel.sek);
            }


        }



        static void AdminMenu()
        {
            Menu CreateAcoountMenu = new Menu(new string[] { "Skapa användare", "Gå tillbaka" });
            bool showMenu = true;
            while (showMenu)
            {

                switch (CreateAcoountMenu.UseMenu())
                {
                    case 0:
                        // Get model of new user
                        BankUserModel newUser = new BankUserModel();
                        Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                                    RegexOptions.CultureInvariant | RegexOptions.Singleline);
                        bool isValidEmail = false;
                        do
                        {
                            Console.Clear();
                            Console.CursorTop = 0;
                            Console.Write($"Ange förnamn: ");
                            newUser.first_name = Console.ReadLine();
                            Console.Write($"Ange efternamn: ");
                            newUser.last_name = Console.ReadLine();

                            // Error handling for incorrect name.
                            bool isValidName = newUser.first_name.Length > 0 && newUser.last_name.Length > 0 ? true : false;
                            if (!isValidName)
                            {
                                Console.WriteLine($"Du måste ange både för och efternamn!");
                                Console.ReadLine();
                                continue;
                            }

                            Console.Write($"Ange mail: ");
                            newUser.email = Console.ReadLine();
                            
                            // Error handling for incorrect email.
                            isValidEmail = regex.IsMatch(newUser.email);
                            if (!isValidEmail)
                            {
                                Console.WriteLine($"Mailadressen du angett ({newUser.email}) är felaktig. Var god försök igen.");
                                Console.ReadLine();
                            }
                        }
                        while (!isValidEmail);

                        newUser.branch_id = 3;

                        //Generate pin for user
                        Random random = new Random();
                        newUser.pin_code = Convert.ToString(random.Next(1000, 9999));

                        DataAccess.CreateUser(newUser);
                        Console.WriteLine($"\nAnvändare har skapats\nMail: {newUser.email}\nNamn: {newUser.first_name} {newUser.last_name}\nPinkod: {newUser.pin_code} ");


                        Console.ReadLine();


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