using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public static class Helper
    {
        public static string FormatString(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }
    }
}
