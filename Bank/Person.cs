using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTesting
{
    public static class Person
    {
        public static int id { get; set; }
        public static int attempts { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Email { get; set; }
        public static string PinCode { get; set; }
        public static string FullName
        {
            get { return $"{FirstName} + {LastName}"; }
        }
    }
}
