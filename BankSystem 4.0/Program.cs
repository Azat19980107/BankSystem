namespace BankSystem_4._0
{
    class BankAccount
    {
        public string Name;
        public int Id;
        public decimal Balance {  get; set; }
    }
    internal class Program
    {
        static BankAccount CreateAccount(string name, int id)
        {
            return new BankAccount { Name = name, Id = id };
        }
        static void Main(string[] args)
        {
            List<BankAccount> accounts = new List<BankAccount>();

            while (true)
            {
                int.TryParse(Console.ReadLine(), out int button);
                if (button == 1)
                {
                    Console.WriteLine("Введите имя");
                    string name = Console.ReadLine();
                    Console.WriteLine("Введите ID");
                    int.TryParse(Console.ReadLine(), out int id);
                    accounts.Add(CreateAccount(name, id));
                }
                else if (button == 2)
                {
                    Console.WriteLine("Посмотреть аккаунты");

                    foreach (var account in accounts)
                    {
                        Console.WriteLine($"{account.Name}, Баланс: {account.Balance}");
                    }
                }
                else if(button == 0)
                {
                    break;
                }

                Console.ReadLine();
                  
            }
        }
    }
}
