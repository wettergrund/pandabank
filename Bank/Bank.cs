using System.Text.RegularExpressions;

namespace Bank
{
    internal class Bank
    {
        //temporary placement for moving ascii art before the user logs in
        // Shows a main menu to the user with options to login, or to exit the program
        public void ShowMenu()
        {
            Menu MainMenu = new Menu(new string[] { "Logga in", "Stäng programmet" });
            bool showMenu = true;
            while (showMenu)
            {
                switch (MainMenu.UseMenu())
                {
                    case 0:
                        if (Login.LoginChecker())
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
            Menu UserMenu = new Menu(new string[] { "Konton", "Betala och Överföra", "Utgiftskollen", "Lån", "Logga ut" });
            // Menu for admin user
            bool isAdmin = DataAccess.AdminAccess();
            if (isAdmin)
            {
                UserMenu = new Menu(new string[] { "Konton", "Betala och Överföra", "Utgiftskollen", "Lån", "Logga ut", "Admin" });
            }
            bool showMenu = true;
            while (showMenu)
            {
                int index = UserMenu.UseMenu();
                if (UserMenu.GetMenuItem() == "Logga ut")
                {
                    break;
                }
                else
                {
                    MenuOptions(index);
                }
            }
        }

        private void MenuOptions(int index)
        {
            Loan Loan = new Loan();
            Menu OptionsMenu = new Menu();
            bool showMenu = true;
            while (showMenu)
            {
                switch (index)
                {
                    case 0:
                        OptionsMenu.MenuItems = new string[] { "Saldon", "Skapa konto", "Ta bort konto", "Gå tillbaka" };
                        showMenu = AccountOptions(OptionsMenu.UseMenu());
                        break;
                    case 1:
                        OptionsMenu.MenuItems = new string[] { "Överföringar", "Sätt in pengar", "Ta ut pengar", "Gå tillbaka" };
                        showMenu = TransactionOptions(OptionsMenu.UseMenu());
                        break;
                    case 2:
                        showMenu = TransactionLog();
                        break;
                    case 3:
                        showMenu = Loan.loanIntrestRate();
                        break;
                    case 4:
                        showMenu = false;
                        break;
                    case 5:
                        showMenu = AdminMenu();
                        break;
                }
            }
        }

        private bool AccountOptions(int index)
        {
            switch (index)
            {
                case 0:
                    ShowAccountBalance();
                    break;
                case 1:
                    CreateAccount();
                    break;
                case 2:
                    DeleteAccount();
                    break;
                case 3:
                    return false;
            }
            return true;
        }
        //CaseManager case = new CaseManager();
        //case.size = array.length
        //case(Transfer(), Deposit(), Withdraw())
        private bool TransactionOptions(int index)
        {
            BankTransfers BankTransactions = new BankTransfers();
            switch (index)
            {
                case 0:
                    BankTransactions.Transfer();
                    break;
                case 1:
                    Deposit();
                    break;
                case 2:
                    Withdraw();
                    break;
                case 3:
                    return false;
            }
            return true;
        }

        // Calls the method to create a menu to show account balances
        public void ShowAccountBalance()
        {
            Menu BalanceMenu = new Menu();
            BalanceMenu.CreateTransferMenu();
            BalanceMenu.UseMenu();
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

        public bool AdminMenu()
        {
            Menu CreateAccountMenu = new Menu(new string[] { "Skapa användare", "Lås upp användare", "Gå tillbaka" });
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
                            newUser.first_name = Helper.FormatString(Console.ReadLine());
                            Console.Write($"Ange efternamn: ");
                            newUser.last_name = Helper.FormatString(Console.ReadLine());

                            // Error handling for incorrect name.
                            bool isValidName = newUser.first_name.Length > 0 && newUser.last_name.Length > 0 ? true : false;
                            if (!isValidName)
                            {
                                Console.WriteLine($"Du måste ange både för och efternamn!");
                                Console.ReadLine();
                                continue;
                            }

                            Console.Write($"Ange mail: ");
                            newUser.email = Helper.FormatString(Console.ReadLine());

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
                        Menu LockedUsers = new Menu();
                        int userID = LockedUsers.CreateLockedMenu();
                        if (userID == -1)
                        {
                            Console.WriteLine("Ingen användare är låst");
                            Console.ReadLine();
                            break;
                        }
                        LockedUsers.UseMenu();
                        DataAccess.UnlockUser(userID);
                        Console.WriteLine("Användarens konto är nu upplåst.");
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine("Gå tillbaka");
                        showMenu = false;
                        break;
                }
            }
            return false;
        }

        //Method to create an account
        private void CreateAccount()
        {
            BankAccountModel newAccount = new BankAccountModel();
            Console.Write("Kontonamn: ");
            string currencyName = "SEK;";
            string accountName = Helper.FormatString(Console.ReadLine());
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
                    newAccount.interest_rate = Helper.AccountType();
                    newAccount.currency_id = Helper.CurrencyType();
                    if (newAccount.currency_id == 2)
                    {
                        currencyName = "USD";
                    }
                    Console.WriteLine("Hur mycket pengar vill du sätta in?");
                    Console.Write(accountName + ": ");

                    string? amount = Console.ReadLine().Replace(",", ".");
                    bool succAmount = decimal.TryParse(amount, out decimal accValue);

                    if (!Helper.CheckChange(amount))
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

        private void DeleteAccount()
        {
            Menu AccountMenu = new Menu();
            List<BankAccountModel> accounts = AccountMenu.CreateTransferMenu();
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
                        Console.WriteLine("Fel pinkod");
                        Console.ReadKey();
                    }
                }
            }
        }

        public static bool TransactionLog()
        {
            Menu TransactionMenu = new Menu();
            List<BankAccountModel> options = TransactionMenu.CreateTransferMenu();
            int selectedRow;
            while (true)
            {
                TransactionMenu.Output = "Från: ";
                selectedRow = TransactionMenu.UseMenu();
                if (TransactionMenu.GetMenuItem() == "Gå tillbaka")
                {
                    return false;
                }
                else
                {
                    List<BankTransactionModel> transactions = DataAccess.GetTransactions(options[selectedRow].id);
                    foreach (var transaction in transactions)
                    {
                        transaction.SetTransactionName(options[selectedRow].id);
                        Console.WriteLine("-------------------------");
                        Console.Write(transaction.transaction_name + "\nSumma: ");
                        Console.Write(transaction.GetSignedAmount(options[selectedRow].id) + "\n");
                        Console.ResetColor();
                        if (transaction.transaction_name != "Insättning" && transaction.transaction_name != "Uttag")
                        {
                            Console.WriteLine("Från: " + transaction.from_account_name + "\nTill: " + transaction.to_account_name);
                        }
                        else if (transaction.transaction_name == "Uttag")
                        {
                            Console.WriteLine("Från: " + transaction.from_account_name);
                        }
                        else
                        {
                            Console.WriteLine("Till: " + transaction.to_account_name);
                        }
                        Console.WriteLine("Datum: " + transaction.timestamp);
                        Console.WriteLine("-------------------------");
                    }
                    Console.ReadKey();
                }
            }
        }
        public void Deposit()
        {
            //Fixa så menu val ligger brevid varanda ?
            Menu DepositMenu = new Menu();
            List<BankAccountModel> accounts = DepositMenu.CreateTransferMenu();
            int selectedAccount;
            decimal depositMoney;

            DepositMenu.Output = "Välj konto.";
            selectedAccount = DepositMenu.UseMenu();
            if (selectedAccount != DepositMenu.MenuItems.Length - 1)
            {
                Console.WriteLine("Ange hur mycket du vill du vill sätta in.");
                string amount = Console.ReadLine().Replace(",", ".");
                bool success = decimal.TryParse(amount, out decimal userInput);
                if (success && Helper.CheckChange(amount))
                {
                    if (userInput > 0)
                    {
                        depositMoney = userInput;
                        Console.WriteLine("Du har satt in " + userInput + ":- på konto [" + accounts[selectedAccount].name + "]");
                        Console.ReadKey();
                        DataAccess.Deposit(accounts[selectedAccount].id, depositMoney);
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt belopp");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("Någonting gick fel. Försök igen.");
                    Console.ReadKey();
                }
            }

        }
        public void Withdraw()
        {
            decimal withdrawMoney = 0;
            Menu WithdrawMenu = new Menu();
            List<BankAccountModel> accounts = WithdrawMenu.CreateTransferMenu();
            WithdrawMenu.Output = "Välj Konto";
            int selectedAccount = WithdrawMenu.UseMenu();
            if (selectedAccount != WithdrawMenu.MenuItems.Length - 1)
            {
                Menu withdrawAmount = new Menu(new string[] { "100:-", "200:-", "500:-", "1000:-", "Ange egen summa." });
                switch (withdrawAmount.UseMenu())
                {
                    case 0:
                        withdrawMoney = 100;
                        break;
                    case 1:
                        withdrawMoney = 200;
                        break;
                    case 2:
                        withdrawMoney = 500;
                        break;
                    case 3:
                        withdrawMoney = 1000;
                        break;
                    case 4:
                        Console.WriteLine("Ange hur mycket du vill du vill dra ut.");
                        string amount = Console.ReadLine().Replace(",", ".");
                        bool success = decimal.TryParse(amount, out decimal userInput);
                        if (success && Helper.CheckChange(amount))
                        {
                            if (userInput > 0)
                            {
                                withdrawMoney = userInput;
                            }
                            else
                            {
                                Console.WriteLine("Ogiltigt belopp");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Någonting gick fel. Försök igen.");
                            Console.ReadKey();
                        }
                        break;
                    case 5:
                        break;
                }
                if (withdrawMoney > 0)
                {
                    DataAccess.Withdraw(accounts[selectedAccount].id, withdrawMoney);
                    Console.WriteLine("Du tog ut " + withdrawMoney + ":- från [" + accounts[selectedAccount].name + "]\nTryck valfri knapp för fortsätta");
                    Console.ReadKey();
                }
            }
        }
        static void ConsoleDraw(IEnumerable<string> lines, int x, int y)
        {
            if (x > Console.WindowWidth) return;
            if (y > Console.WindowHeight) return;

            var trimLeft = x < 0 ? -x : 0;
            int index = y;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            var linesToPrint =
                from line in lines
                let currentIndex = index++
                where currentIndex > 0 && currentIndex < Console.WindowHeight
                select new
                {
                    Text = new String(line.Skip(trimLeft).Take(Math.Min(Console.WindowWidth - x, line.Length - trimLeft)).ToArray()),
                    X = x,
                    Y = y++
                };

            Console.Clear();
            foreach (var line in linesToPrint)
            {
                Console.SetCursorPosition(line.X, line.Y);
                Console.Write(line.Text);
            }
        }
        public static void LoginArt()
        {
            { //ascii art for when the user logs in
                Console.CursorVisible = false;

                var arr = new[]
                {

@"                              _,add8ba, ",
@"                            ,d888888888b, ",
@"                           d8888888888888b                        _,ad8ba,_ ",
@"                          d888888888888888)                     ,d888888888b, ",
@"                          I8888888888888888 _________          ,8888888888888b ",
@"                __________`Y88888888888888P           baaa,__ ,888888888888888, ",
@"            ,adP           9888888888P  ^                 ^  Y8888888888888888I ",
@"         ,a8 ^           ,d888P 888P^        BY:                ^ Y8888888888P' ",
@"       ,a8^            ,d8888              Jonas                  ^Y8888888P' ",
@"      a88'           ,d8888P                Christopher              I88P ^ ",
@"    ,d88'           d88888P                    Leo                      b, ",
@"   ,d88'           d888888                      Zak                     b, ",
@"  ,d88'           d888888I                        Morgan                 b,",
@"  d88I           ,8888888             ___                                `b, ",
@" ,888'           d8888888          ,d88888b,              ____            `b, ",
@" d888           ,8888888I         d88888888b,           ,d8888b,           `b ",
@",8888           I8888888I        d8888888888I          ,88888888b           8, ",
@"I8888           88888888b       d88888888888           8888888888b          8I ",
@"d8886           888888888       Y888888888P            Y8888888888,        ,8b ",
@"88888b          I88888888b       Y8888888^              Y888888888I        d88, ",
@"Y88888b          888888888b,      ^   ^                 Y8888888P'       d888I ",
@" 888888b         88888888888b,                            Y8888P^        d88888 ",
@" Y888888b       ,8888888888888ba,_          _______          ^        ,d888888 ",
@" I8888888b,    ,888888888888888888ba,_     d88888888b               ,ad8888888I ",
@" `888888888b,  I8888888888888888888888b,    ^^Y888P^^      ____.,ad88888888888I ",
@"  88888888888b,`888888888888888888888888b,     ~~      ad888888888888888888888' ",
@"  8888888888888698888888888888888888888888b_,ad88ba,_,d88888888888888888888888 ",
@"  88888888888888888888888888888888888888888b,`^^^ d8888888888888888888888888I",
@"  8888888888888888888888888888888888888888888baaad888888888888888888888888888'",
@"  Y8888888888888888888888888888888888888888888888888888888888888888888888888P",
@"  I888888888888888888888888888888888888888888888P^  ^Y8888888888888888888888'",
@"   Y88888888888888888P88888888888888888888888888'     ^88888888888888888888I ",
@"   Y8888888888888888  8888888888888888888888888       8888888888888888888P ",
@"     Y888888888888888   888888888888888888888888,     ,888888888888888888P ",
@"      Y88888888888888b   88888888888888888888888I     I888888888888888888 ",
@"        Y8888888888888b   8888888888888888888888I     I88888888888888888 ",
@"         Y88888888888P    888888888888888888888b     d8888888888888888 ",
@"           ^^^^^^^^^^      Y88888888888888888888,    888888888888888P ",
@"                             8888888888888888888b,   Y888888888888P^ ",
@"                              Y888888888888888888b    Y8888888P^ ",
@"                                Y8888888888888888P            ^ ",
@"                                   YY88888888888P ",
@"                                       ^^^^^^^^^^ ",

            @"                                             ",
            @"    \ \        / / | |                         ",
            @"     \ \  /\  / /__| | ___ ___  _ __ ___   ___  ",
            @"      \ \/  \/ / _ \ |/ __/ _ \| '_ ` _ \ / _ \ ",
            @"       \  /\  /  __/ | (_| (_) | | | | | |  __/ ",
            @"     ___\/__\/ \___|_|\___\___/|_| |_| |_|\___| ",
            @"    |__   __|                                   ",
            @"       | | ___                                  ",
            @"       | |/ _ \                                 ",
            @"       | | (_) |                                ",
            @"     __|_|\___/            _        _                _ ",
            @"     |  __ \              | |     | |               | | ",
            @"     | |__) |_ _ _ __   __| | __ _| |__   __ _ _ __ | | _____ _ __  ",
            @"     |  ___/ _` | '_ \ / _` |/ _` | '_ \ / _` | '_ \| |/ / _ \ '_ \ ",
            @"     | |  | (_| | | | | (_| | (_| | |_) | (_| | | | |   <  __/ | | |",
            @"     |_|   \__,_|_| |_|\__,_|\__,_|_.__/ \__,_|_| |_|_|\_\___|_| |_|",

        };

                var maxLength = arr.Aggregate(0, (max, line) => Math.Max(max, line.Length));
                var x = Console.BufferWidth / 2 - maxLength / 2;
                for (int y = -arr.Length; y < Console.WindowHeight + arr.Length; y++)
                {
                    ConsoleDraw(arr, x, y);
                    Thread.Sleep(100);
                }
            };
        }
    }
}


