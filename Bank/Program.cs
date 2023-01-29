using DatabaseTesting;

namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ShowMenu();
        }

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

        private static void ShowUserMenu()
        {
            Menu UserMenu = new Menu(new string[] { "Konton/Saldon", "Överför pengar mellan konton", "Logga ut" });
            bool showMenu = true;
            while (showMenu)
            {
                switch (UserMenu.UseMenu())
                {
                    case 0:
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