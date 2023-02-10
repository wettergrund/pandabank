using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    internal class Loan
    {
        public void loanIntrestRate()
        {    // todo : fixa lån variable , 
            //Get data for logged in user , used to get balance
            BankLoanModel newLoan = new BankLoanModel();
            decimal totalBalance = DataAccess.CheckTotalBalance(Person.id);
            double loanAmountRate = 0;
            decimal loanAmount = 0;
            Console.WriteLine("Hur mycket vill du låna?");
            Menu loanMenu = new Menu(new string[] { "1000:-", "10000:-", "100000:-", "Eget belopp", "Gå tillbaka" });
            switch (loanMenu.UseMenu())
            {
                case 0:
                    loanAmount = 1000;
                    loanAmountRate = 30;
                    newLoan.name = "Blancolån";
                    break;
                case 1:
                    loanAmount = 10000;
                    loanAmountRate = 15;
                    newLoan.name = "Privatlån";
                    break;
                case 2:
                    loanAmount = 100000;
                    if (totalBalance * 5 >= loanAmount)
                    {
                        loanAmountRate = 7;
                        newLoan.name = "Privatlån";
                    }
                    else
                    {
                        Console.WriteLine("Du har förlite pengar för att låna mängden pengar");
                    }
                    break;
                case 3:
                    decimal userLoan;
                    Console.Write("Ange belop du vill låna:");
                    decimal.TryParse(Console.ReadLine(), out userLoan);
                    loanAmount = userLoan;
                    if (totalBalance * 5 >= loanAmount)
                    {
                        loanAmountRate = 5;
                        newLoan.name = "Privatlån";
                    }
                    else
                    {
                        Console.WriteLine("Du har för lite pengar för att låna mängden pengar");
                    }
                    break;
                case 4:
                    //Gå tillbaka
                    break;
            }
            if (loanAmount > 0)
            {
                newLoan.amount = loanAmount;
                newLoan.interest_rate = loanAmountRate;
                List<BankUserModel> pincheck = DataAccess.GetUserData(Person.id);
                Menu confirm = new Menu(new string[] { "Ja", "Nej" });
                Console.WriteLine("Du har tagit ett " + newLoan.name + "på " + loanAmount + ":- med en räta på " + loanAmountRate + "%.");
                confirm.Output = "Du har tagit ett " + newLoan.name + " på " + loanAmount + ":- med en räta på " + loanAmountRate + "%.\nGodkänner du detta lån?";
                switch (confirm.UseMenu())
                {
                    case 0:
                        Console.Write("Skriv in din pinkod för att bekräfta lånet:");
                        string pincode = Helper.MaskPincodeData();
                        if (pincode == pincheck.First().pin_code)
                        {
                            DataAccess.UpdateLoanAmount(newLoan, Person.id);
                            Console.WriteLine("Lån godkänt. Pengarna sätts in inom 5 arbets dagar.");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\nFel pinkod,du sickas tillbaka till menyvalen.");
                            Console.ReadKey();
                        }
                        break;
                    case 1:
                        break;
                }

            }
        }
    }
}
