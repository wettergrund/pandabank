using DatabaseTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    internal class UserTransfers
    {
        readonly Menu TransferMenu = new Menu(new string[] { "Välj konto:", "Till:", "Summa:", "Överför" ,"Gå tillbaka" });
        int fromID;
        string? reciever;
        decimal amount;
        public bool Transfer()
        {
            bool isTransfering = true;
            while (isTransfering)
            {
                switch (TransferMenu.UseMenu())
                {
                    case 0:
                        FromAccount();
                        break;
                    case 1:
                        TransferMenu.SetMenuItem("Till:", 1);
                        TransferMenu.PrintSystem();
                        ToUser();
                        break;
                    case 2:
                        TransferMenu.SetMenuItem("Summa:", 2);
                        TransferMenu.PrintSystem();
                        AmountToTransfer();
                        break;
                    case 3:
                        BeginTransaction();
                        break;
                    case 4:
                        return false;
                }
            }
            return true;
        }

        private void FromAccount()
        {
            Menu fromAccountMenu = new Menu();
            List<BankAccountModel> options = fromAccountMenu.CreateTransferMenu(Person.id);
            int selectedRow;
            while (true)
            {
                fromAccountMenu.Output = "Från: ";
                selectedRow = fromAccountMenu.UseMenu();
                if (selectedRow == fromAccountMenu.GetMenu().Length - 1)
                {
                    break;
                }
                else
                {
                    TransferMenu.SetMenuItem("Välj konto: " + options[selectedRow].name, 0);
                    fromID = options[selectedRow].id;
                    break;
                }
            }
        }

        private void ToUser()
        {
            // Kolla först om emailen är korrekt skriven
            // Kolla sedan om mailen existerar
            bool isEmail = true;
            while (isEmail)
            {
                TransferMenu.MoveCursorRight();
                isEmail = ValidateEmail(Console.ReadLine());
            }
        }

        private void AmountToTransfer()
        {
            // Checka ifall mängden pengar finns på valda kontot
            // Varna annars användaren
            Console.WriteLine("Summa?");
            string answer = Console.ReadLine();
            bool success = decimal.TryParse(answer, out amount);
            if(success)
            {
                bool enoughFunds = DataAccess.CheckAccountFunds(fromID, amount);
                if (enoughFunds)
                {
                    TransferMenu.SetMenuItem("Summa: " + amount.ToString(), 2);
                }
                else
                {
                    amount = 0;
                    Warning("test", "Inte tillräcklig täckning på kontot. Försök igen.");
                }
            }
        }

        private void BeginTransaction()
        {
            // Be användaren bekräfta med pinkod för att föra över pengarna.
            Console.WriteLine("Transaction started.");
            Console.ReadKey();
        }

        private void ResetTransferData()
        {
            TransferMenu.SetMenuItem("Välj Konto:", 0);
            TransferMenu.SetMenuItem("Till:", 1);
            TransferMenu.SetMenuItem("Summa:", 2);
            TransferMenu.PrintSystem();
        }

        //Moves the cursor to the bottom, prints the given error/warning message to the user
        //Resets the menu output and moves the cursor back
        private void Warning(string menuItem, string errorMessage)
        {
            TransferMenu.MoveCursorBottom();
            Console.WriteLine(errorMessage);
            //TransferMenu.SetMenuItem(menuItem, TransferMenu.SelectIndex);
            Console.ReadKey(true);
            TransferMenu.MoveCursorTop();
        }

        private bool ValidateEmail(string email)
        {
            if (Regex.IsMatch(email, @"^[a-zA-Z0-9_@.]+$"))
            {
                reciever = Helper.FormatString(email);
                if(DataAccess.CheckUserExists(reciever))
                {
                    TransferMenu.SetMenuItem("Till:" + reciever, 1);
                    return false;
                }
                else
                {
                    Warning("test", "Skriv in en existerande användare!");
                }
            }
            ResetTransferData();
            //Warning("Email:", "Otillåtna tecken. Försök igen.");
            return true;
        }
    }
}
