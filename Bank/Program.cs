namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Förklaring om klassen - Ta bort detta sen
            // Man kan skapa nya menyer så här, så får man ett returnerat index på valt item
            // Väljer man t.ex Exit - Så får man tillbaka "2"
            // Och kan sedan användas i en switch case eller liknande
            // Hur man får ett index värde visas nedan
            Menu TestMenu = new Menu(new string[] { "Log in", "Test Item or something", "Tester", "Exit" });

            // Men i vanliga fall blir det väl redan färdiga arrays som man skapar menyer
            // Vilket också fungerar
            string[] test = { "Konton", "Överföring", "Logga ut" };
            Menu SecondTest = new Menu(test); // Går att länka in en array, eller skapa en

            //Man kan även sätta in andra menyer i efterhand
            SecondTest.SetMenu(new string[] { "Test1", "Test2", "Test3" });

            //För att visa menyerna så finns det både en funktion för att printa samt att använda menyn
            int index = TestMenu.UseMenu(); // Denna tillåter för användningen av konsollen och returnerar då ett index
            Console.WriteLine("Your chosen index {0}", index);
            Console.ReadKey();

            SecondTest.UseMenu();

            //Kontrollerna för menyn är Piltangeterna, Space och Enter.
            //Men även bokstäver fungerar, ifall ett menyval matchar sagd bokstav så hoppar den dit
            #endregion
        }
    }
}