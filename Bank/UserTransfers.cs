using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bank
{
    public class UserTransfers
    {
        readonly static Menu TransferMenu = new Menu(new string[] { "Välj konto:", "Till:", "Summa:", "Överför", "Gå tillbaka" });
        BankTransactionModel transaction = new BankTransactionModel();
        string? toEmail;
        int selectedAccount;
        public void Transfer()
        {
            ResetTransferData();
            TransferMenu.SelectIndex = 0;
            bool isTransfering = true;
            while (isTransfering)
            {
                switch (TransferMenu.UseMenu())
                {
                    case 0:
                        FromAccount();
                        break;
                    case 1:
                        ResetRow("Till:");
                        SelectTransferType();
                        break;
                    case 2:
                        ResetRow("Summa:");
                        GetAmountToTransfer();
                        break;
                    case 3:
                        Transaction();
                        break;
                    case 4:
                        isTransfering = false;
                        break;
                }
            }
        }

        private void FromAccount()
        {
            Menu fromAccountMenu = new Menu();
            List<BankAccountModel> options = fromAccountMenu.CreateTransferMenu(selectedAccount);
            int selectedRow;
            while (true)
            {
                fromAccountMenu.Output = "Från: ";
                selectedRow = fromAccountMenu.UseMenu();
                if (fromAccountMenu.GetMenuItem() == "Gå tillbaka")
                {
                    break;
                }
                else
                {
                    TransferMenu.SetMenuItem("Valt konto: " + options[selectedRow].name + " - " + options[selectedRow].balance, 0);
                    transaction.from_account_id = options[selectedRow].id;
                    selectedAccount = options[selectedRow].id;
                    break;
                }
            }
        }

        private void SelectTransferType()
        {
            Menu TransferTypeMenu = new Menu(new string[] { "Egna konton", "Annan användare" });
            TransferTypeMenu.Output = "Välj typen av överföring.";
            switch (TransferTypeMenu.UseMenu())
            {
                case 0:
                    ToAccount();
                    break;
                case 1:
                    TransferMenu.PrintMenu();
                    ToUser();
                    break;
                case 2:
                    break;
            }
        }

        private void ToAccount()
        {
            Menu toAccountMenu = new Menu();
            List<BankAccountModel> options = toAccountMenu.CreateTransferMenu(selectedAccount);
            int selectedRow;
            while (true)
            {
                toAccountMenu.Output = "Till: ";
                selectedRow = toAccountMenu.UseMenu();
                if (toAccountMenu.GetMenuItem() == "Gå tillbaka")
                {
                    break;
                }
                else
                {
                    TransferMenu.SetMenuItem("Valt konto: " + options[selectedRow].name + " - " + options[selectedRow].balance, 1);
                    transaction.to_account_id = options[selectedRow].id;
                    selectedAccount = options[selectedRow].id;
                    toEmail = "";
                    break;
                }
            }
        }

        // Prompts user to enter the recieving account, and then checks if user exists
        private void ToUser()
        {
            bool isEmail = true;
            while (isEmail)
            {
                TransferMenu.MoveCursorRight();
                isEmail = ValidateEmail(Console.ReadLine());
            }
            if (Person.id == DataAccess.GetUserID(toEmail))
            {
                Console.WriteLine("Välj en annan användare än dig själv.");
                toEmail = "";
                Console.ReadKey();
            }
            else
            {
                int transferID = DataAccess.GetUserID(toEmail);
                if(transferID > 0)
                {
                    transaction.to_account_id = DataAccess.GetAccountID(transferID);
                }
                else
                {
                    Warning("Mottagaren saknar konton att ta emot transaktionen med.");
                }
            }
        }
        // Prompts user to enter the amount,
        // then checks if their chosen account has enough funds to cover the transaction
        private void GetAmountToTransfer()
        {
            TransferMenu.MoveCursorRight();
            string answer = Console.ReadLine().Replace(",", ".");
            bool success = decimal.TryParse(answer, out decimal amount);
            if (success && Helper.CheckChange(answer))
            {
                bool enoughFunds = DataAccess.CheckAccountFunds(transaction.from_account_id, amount);
                if (enoughFunds)
                {
                    transaction.amount = amount;
                    TransferMenu.SetMenuItem("Summa:" + amount.ToString(), 2);
                }
                else
                {
                    transaction.amount = 0;
                    Warning("Inte tillräcklig täckning på kontot. Försök igen.");
                }
            }
            else
            {
                Warning("Ange endast siffror och ej fler än två decimaler.");
            }
        }
        // If all data input is valid, the transaction will go through
        private void Transaction()
        {
            if (transaction.amount > 0 && transaction.from_account_id > -1 && transaction.to_account_id > -1)
            {
                // Checks that all the required variables have valid values
                if (CheckPincode())
                {
                    DataAccess.TransferToUser(transaction, "Kontoöverföring");
                    ResetTransferData();
                }
                else
                {
                    Console.WriteLine("Fel pinkod. Försök igen!");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Dubbelkolla så att all data ovan är ifyllt.");
                Console.ReadKey();
            }
        }
        // Promps the user to enter their pincode, and then checks if is correct
        private bool CheckPincode()
        {
            bool isValid = false;

            Console.Write("Bekräfta överföringen med pinkod: ");
            while (!isValid) // Will run until a pin consisting of only numbers is entered
            {
                string pin = Helper.MaskPincodeData();
                isValid = ValidatePincode(pin);
            }
            // If pincode is valid and user hasnt exceeded 3 attempts
            if(isValid && DataAccess.CheckUserInfo(Person.Email, Person.PinCode)) // If pin is correct for the currently logged in user
            {
                return true;
            }
            return false;
        }
        // Resets all the data input
        private void ResetTransferData()
        {
            transaction.from_account_id = -1;
            transaction.to_account_id = -1;
            toEmail = "";
            transaction.amount = -1;
            TransferMenu.SetMenuItem("Välj Konto:", 0);
            TransferMenu.SetMenuItem("Till:", 1);
            TransferMenu.SetMenuItem("Summa:", 2);
            TransferMenu.PrintMenu();
        }
        // Resets a specific row
        private void ResetRow(string row)
        {
            TransferMenu.SetMenuItem(row, TransferMenu.SelectIndex);
            TransferMenu.PrintMenu();
        }
        //Moves the cursor to the bottom, prints the given error/warning message to the user
        //Resets the menu output and moves the cursor back
        private void Warning(string errorMessage)
        {
            TransferMenu.MoveCursorBottom();
            Console.WriteLine(errorMessage);
            Console.ReadKey(true);
            TransferMenu.MoveCursorRight();
        }
        // When called this will check if email consists of valid symbols
        // And then check if the email is linked to an existing user
        private bool ValidateEmail(string email)
        {
            if (Regex.IsMatch(email, @"^[a-zA-Z0-9_@.]+$"))
            {
                toEmail = Helper.FormatString(email);
                if (DataAccess.CheckUserExists(toEmail))
                {
                    TransferMenu.SetMenuItem("Till:" + toEmail, 1);
                    return false;
                }
                else
                {
                    TransferMenu.SetMenuItem("Till:", TransferMenu.SelectIndex);
                    TransferMenu.PrintMenu();
                    toEmail = "";
                    Warning("Skriv in en existerande användare!");
                }
            }
            else
            {
                toEmail = "";
                Warning("Otillåtna tecken. Försök igen.");
                TransferMenu.PrintMenu();
            }
            return true;
        }
        //Checks if password only contains integers
        public bool ValidatePincode(string pincode)
        {
            bool success = int.TryParse(pincode, out int result);
            if (success && pincode.Length <= 4)
            {
                Person.PinCode = result.ToString();
                return true;
            }
            Console.WriteLine("Endast nummer är tillåtna. Försök igen.");
            return false;
        }
    }
}
