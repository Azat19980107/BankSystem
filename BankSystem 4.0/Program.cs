namespace BankSystem_4._0
{
    class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public override string ToString()
        {
            return $"{Name}, {Id}, Баланс: {Balance}";
        }

    }
    class Bank
    {
        List <BankAccount> accounts = new List <BankAccount> ();

        public void CreateAccount (string name, int id)
        {
            accounts.Add(new BankAccount
            {
                Id = id,
                Name = name,
            });
        }

        public void ShowInfo ()
        {
            foreach (var account in accounts)
            {
                Console.WriteLine(account);
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            
            while (true)
            {
                Console.WriteLine(
                    "1 - Создать аккаунт\n" +
                    "2 - Посмотреть аккаунты");

                if (!int.TryParse(Console.ReadLine(), out int button))
                {
                    Console.WriteLine("Введите число");
                    continue;
                }

                switch (button)
                {
                    case 1:

                        Console.WriteLine("Введите свое имя");
                        string customerName = Console.ReadLine();
                        Console.WriteLine("Введите ID");
                        bool isRunning = true;
                        while (isRunning)
                        {
                            bool isNumber = int.TryParse(Console.ReadLine(), out int id);

                            if(!isNumber)
                            {
                                Console.WriteLine("Введите число");
                                continue;
                            }

                            someBank.CreateAccount(customerName, id);

                            Console.WriteLine("Аккаунт создан");
                            
                            isRunning = false;
                            
                        }


                        break;

                    case 2:

                        someBank.ShowInfo();
                        break;
                }
                
            }
        }
    }
}
