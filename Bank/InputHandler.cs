using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    // A class that takes in and checks user input
    internal class InputHandler
    {
        ConsoleKey keyPressed;
        ConsoleKey key1, key2, key3, key4;
        int index;

        public InputHandler()
        {
            //Allowed input
            key1 = ConsoleKey.UpArrow;
            key2 = ConsoleKey.DownArrow;
            key3 = ConsoleKey.Enter;
            key4 = ConsoleKey.Spacebar;
        }
        public int GetIndex()
        {
            return index;
        }

        // Listens for input from the user
        // Once valid input is given, returns the valid keypress info
        public ConsoleKey ReadInput(string[] input, int position)
        {
            // A loop that keeps listening to userInput
            // Until correct input is given, then returns said key
            do
            {
                keyPressed = Console.ReadKey(true).Key;
                // If a matching letter is found, sets that as the index
                if ((index = CheckLetter(input, position, keyPressed)) >= 0)
                {
                    return keyPressed;
                }
            } while (keyPressed != key1 && keyPressed != key2 && keyPressed != key3 && keyPressed != key4);
            return keyPressed;
        }
        // A method to see if userinput matches any of the menu items
        // And returns a matching index if found, else returns -1
        private static int CheckLetter(string[] input, int position, ConsoleKey key)
        {
            int num = -1;
            // Loops through the array
            for (int i = 0; i < input.Length; i++)
            {
                // Also checks so that we are not already at currently selected index, also checks the first letter on each row for a match to user input 
                // If found, stores the row value
                if (input[i].Substring(0, 1).ToUpper() == key.ToString().ToUpper() && i != position)
                {
                    num = i;
                    break;
                }
            }
            return num;
        }
    }
}