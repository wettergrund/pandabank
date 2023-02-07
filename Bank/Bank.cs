using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    internal class Bank
    {
        // Shows a main menu to the user with options to login, or to exit the program
        public void ShowMenu()
        {
            Menu MainMenu = new Menu(new string[] { "Logga in", "Stäng programmet" });
            Login BankLogin = new Login(); // 
            bool showMenu = true;
            while (showMenu)
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
        private void ShowUserMenu()
        {
            Menu UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton", "Överför pengar mellan användare", "Skapa ett nytt konto", "Ta bort ett konto", "Logga ut" });
            UserTransfers TransferToUser = new UserTransfers();
            bool isAdmin = DataAccess.AdminAccess();
            bool showMenu = true;

            while (showMenu)
            {
                switch (UserMenu.UseMenu())
                {
                    case 0:
                        ShowAccountBalance();
                        break;
                    case 1:
                        MoveMoney();
                        break;
                    case 2:
                        TransferToUser.Transfer();
                        break;
                    case 3:
                        CreateAccount();
                        break;
                    case 4:
                        DeleteAccount();
                        break;
                    case 5:
                        showMenu = false;
                        break;
                }
            }
        }

        // Calls the method to create a menu to show account balances
        public void ShowAccountBalance()
        {
            Menu BalanceMenu = new Menu();
            BalanceMenu.CreateTransferMenu(Person.id);
            BalanceMenu.UseMenu();
        }
        // Method that allows user to send money between their own accounts
        private void MoveMoney()
        {
            int selectedFromId = SelectAccount.FromID();
            if (selectedFromId > 0)
            {
                int selectedToId = SelectAccount.ToID(selectedFromId);
                if (selectedToId > 0)
                {
                    DataAccess.UpdateBalance(selectedFromId, selectedToId);
                }
            }
        }
        //Method to convert currency
        public void CurrencyConvert()
        {
            List<BankAccountModel> bankAccountModels = DataAccess.CurrencyExchange(1);

            foreach (var bankAccountModel in bankAccountModels)
            {
                Console.WriteLine(bankAccountModel.sek);
            }
        }

        private void AdminMenu()
        {
            Menu CreateAccountMenu = new Menu(new string[] { "Skapa användare", "Gå tillbaka" });
            bool showMenu = true;
            while (showMenu)
            {
                switch (CreateAccountMenu.UseMenu())
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
        //Method to creat a savings account
        private void CreateAccount()
        {
            BankAccountModel newAccount = new BankAccountModel();
            Console.Write("Kontonamn: ");
            string currencyName = "SEK;";
            string accountName = Console.ReadLine();
            if (!Regex.IsMatch(accountName, @"^[a-öA-Ö]+$"))
            {
                Console.WriteLine("Du kan bara skapa konton med bokstäver");
                Console.ReadKey();
            }
            else
            {
                newAccount.name = accountName;
                bool success = true;
                do
                {
                    newAccount.interest_rate = AccountType();
                    newAccount.currency_id = CurrencyType();
                    if(newAccount.currency_id == 2)
                    {
                        currencyName = "USD";
                    }
                    Console.WriteLine("Hur mycket pengar vill du sätta in?");
                    Console.Write(accountName + ": ");

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
                    else if (!succAmount)
                    {
                        Console.WriteLine("Var vänligen använde inte bokstäver");
                        Console.ReadKey();
                    }
                    else
                    {
                        if (accValue < 0)
                        {
                            Console.WriteLine("Du kan inte sätt in minus duh");
                            Console.ReadKey();
                        }
                        else
                        {
                            newAccount.balance = accValue;
                            DataAccess.CreateUserAcc(newAccount);
                            Console.WriteLine($"Du har nu skapat kontot: {newAccount.name} med räntan: {newAccount.interest_rate}");
                            Console.WriteLine($"Nuvarande saldo på kontot är: {newAccount.balance} {currencyName}");
                            Console.ReadKey();
                            success = true;
                        }
                    }
                }
                while (!success);
            }
        }

        private double AccountType()
        {
            Menu AccountTypeMenu = new Menu(new string[] {"Personkonto - 0% Ränta", "Sparkonto - 1.25% Ränta","Pensionsfond - 4% Ränta"});
            AccountTypeMenu.Output = "Välj typ av konto: ";
            switch(AccountTypeMenu.UseMenu())
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

        private int CurrencyType()
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

        private void DeleteAccount()
        {
            Menu AccountMenu = new Menu();
            List<BankAccountModel> accounts = AccountMenu.CreateTransferMenu(Person.id);
            List<BankUserModel> pincheck = DataAccess.GetUserData(Person.id);
            int selectedAccount;
            while (true)
            {
                AccountMenu.Output = "Välj konto att radera.";
                selectedAccount = AccountMenu.UseMenu();
                if (selectedAccount == AccountMenu.GetMenu().Length - 1)
                {
                    break;
                }
                else if (accounts[selectedAccount].balance > 0)
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
                        Console.WriteLine("Ditt konto " + accounts[selectedAccount].name + " har raderats.");
                        Console.ReadKey();
                        DataAccess.DeleteUserAcc(accounts[selectedAccount].id);
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
    }
}
