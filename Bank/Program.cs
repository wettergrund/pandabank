using System.Globalization;
using System.Text.RegularExpressions;

namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {   
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Bank bank = new Bank();
            bank.AdminMenu();
            bank.ShowMenu();
        }       
    }
}