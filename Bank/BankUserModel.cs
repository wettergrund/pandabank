namespace Bank
{
    public class BankUserModel
    {
        public BankUserModel()
        {
            role_id = 2;
        }
        public int id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string pin_code { get; set; }
        public string email { get; set; }

        public int role_id { get; set; }

        public int branch_id { get; set; }

        public bool is_admin { get; set; }

        public bool is_client { get; set; }
        public bool is_locked { get; set; }


        public List<BankAccountModel> accounts { get; set; }
    }
}
