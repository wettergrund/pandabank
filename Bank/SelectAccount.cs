using DatabaseTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    internal class SelectAccount
    {
        internal static int FromID()
        {

            /* Return DB ID of the selected account.  */
            Menu fromAccountMenu = new Menu();
            List<BankAccountModel> options = fromAccountMenu.CreateTransferMenu(Person.id, -1);
            int selectedRow = 0;
            while (true)
            {
                selectedRow = fromAccountMenu.UseMenu();
                if (selectedRow == fromAccountMenu.GetMenu().Length - 1)
                {
                    break;
                }
                else
                {
                    return options[selectedRow].id;
                }
            }
            return -1;
        }

        internal static int ToID(int selectedAccount)
        {
            /* Return DB ID of the selected account, will grey out already selected account  */

            Menu toAccountMenu = new Menu();
            List<BankAccountModel> test = toAccountMenu.CreateTransferMenu(Person.id, selectedAccount);
            int selected = toAccountMenu.UseMenu();

            return test[selected].id;
        }
    }
}