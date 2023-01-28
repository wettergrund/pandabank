using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Bank
{
    // A class that handles the creation of menus and allows the user to interact with them
    internal class Menu
    {
        protected string[] menuArr = Array.Empty<string>();
        protected int selectedIndex = 0;

        // Takes an array of strings on class instantiation
        public void AddMenuItems(string[] items)
        {
            menuArr = items;
        }

        // Getter and setter for the selected index
        // SelectedIndex is the currently selected item in the menu
        public int SelectIndex
        {
            get { return selectedIndex; }
            set
            {
                // Checks if index is within bounds
                if (value >= 0 && value < menuArr.Length) 
                {
                    selectedIndex = value;
                }
            }
        }

        // Allows user to change and set new menus - if needed
        public void SetMenu(string[] menu)
        {
            menuArr = menu;
        }

        // Gets account data for given user_id
        // And creates a string to show accounts in the console
        // Also returns string if needed
        public string[] CreateAccountMenu(int userID)
        {
            List<BankAccountModel> currentUser = DataAccess.GetAccountData(userID);
            string[] menuItems = new string[currentUser.Count + 1];

            // Creates a menu array with the users accounts and adds an option to go back at the end
            for (int i = 0; i < currentUser.Count + 1; i++)
            {
                if (i < currentUser.Count)
                {
                    menuItems[i] = currentUser.ElementAt(i).name;
                }
                else
                {
                    menuItems[i] = "Gå tillbaka";
                }
            }
            SetMenu(menuItems);
            return menuItems;
        }

        // A method that prints the menu when called
        private void PrintSystem()
        {
            Console.Clear();
            // Prints out the menu items in the console, and puts brackets around the selected item.
            for (int i = 0; i < menuArr.Length; i++)
            {
                // Checks if the current menu choice is the selected item 
                // Makes the text green and puts brackets around it
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("[ {0} ]", menuArr[i]);
                }
                else
                {
                    Console.ResetColor();
                    Console.WriteLine("  {0}  ", menuArr[i]);
                }
                Console.ResetColor();
            }
        }

        // A method that allows the user to orientate around the menu
        public int UseMenu()
        {
            // Calls for a class that deals with user input
            // Handles validation of input and only returns valid keyinput
            InputHandler menuInput = new InputHandler();
            while(true)
            {
                PrintSystem();
                ConsoleKey userInput = menuInput.ReadInput(menuArr, selectedIndex); // Returns keyinput if valid
                // Moves up and down in the array, depending on the input
                // If user presses enter, breaks the loop and returns currenty selected index
                if (userInput == ConsoleKey.UpArrow)
                {
                    SelectIndex--;
                }
                else if (userInput == ConsoleKey.DownArrow)
                {
                    SelectIndex++;
                }
                else if (userInput == ConsoleKey.Enter || userInput == ConsoleKey.Spacebar)
                {
                    break; // Will break the loop and return index of currently selected item in the menu
                }
                else
                {
                    SelectIndex = menuInput.GetIndex();
                }
                Console.Clear(); // Clears the old menu
                PrintSystem(); // Prints the newly updated menu
            }
            return selectedIndex;
        }
    }
}