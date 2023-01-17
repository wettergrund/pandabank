namespace Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetUserInfo();
        }

        private static void GetAccountBalance()
        {

        }

        private static decimal[][] GetUserInfo()
        {
            string[] UserTextInput;
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "UserInfo.txt"))) 
            {
                 UserTextInput = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "UserInfo.txt"));
            }
            else
            {
                UserTextInput = File.ReadAllLines("../../../UserInfo.txt");
            }

            string[] userInfo;
            decimal[][] pandaGuldMynt = new decimal[UserTextInput.Length][];

            for (int i = 0; i < UserTextInput.Length; i++)
            {
                int count = UserTextInput[i].Split(';').Length;
                pandaGuldMynt[i] = new decimal[count];
                Console.WriteLine("New account");
                for (int j = 0; j < count; j++)
                {
                    userInfo = UserTextInput[i].Split(';');
                    pandaGuldMynt[i][j] = decimal.Parse(userInfo[j]);
                }
            }
            return pandaGuldMynt;
        }
    }
}