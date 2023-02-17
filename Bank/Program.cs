using System.Globalization;

namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Bank.LoginArt(); // Ascii art at launch
            Bank bank = new Bank();
            bank.ShowMenu(); // Runs the bank program
        }
    }
}