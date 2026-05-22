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
        public void Deposite(decimal amount)
        {
            Balance += amount;
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

        public BankAccount FindAccount (int id)
        {
            return accounts.Find(account => account.Id == id);
        }
        
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            BankAccount account = new BankAccount ();
            
            while (true)
            {
                Console.WriteLine(
                    "1 - Создать аккаунт\n" +
                    "2 - Посмотреть аккаунты\n" +
                    "3 - Пополнить баланс");

                if (!int.TryParse(Console.ReadLine(), out int button))
                {
                    Console.WriteLine("Введите число");
                    continue;
                }

                switch (button)
                {
                    case 1:
                        {
                            Console.WriteLine("Введите свое имя");
                            string customerName = Console.ReadLine();
                            Console.WriteLine("Введите ID");
                            bool isRunning = true;
                            while (isRunning)
                            {
                                bool isNumber = int.TryParse(Console.ReadLine(), out int id);

                                if (!isNumber)
                                {
                                    Console.WriteLine("Введите число");
                                    continue;
                                }

                                someBank.CreateAccount(customerName, id);

                                Console.WriteLine("Аккаунт создан");

                                isRunning = false;

                            }


                            break;
                        }
                    case 2:
                        {
                            someBank.ShowInfo();
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Для пополнения баланс введите ID");

                            bool isRunning = true;

                            while (isRunning)
                            {
                                bool isIdNumber = int.TryParse(Console.ReadLine(), out int id);

                                if (!isIdNumber)
                                {
                                    Console.WriteLine("Введите число");
                                    continue;
                                }

                                var foundUser = someBank.FindAccount(id);

                                if (foundUser == null)
                                {
                                    Console.WriteLine("Аккаунт не найден");
                                    continue;
                                }

                                Console.WriteLine("Введите сумму для пополнения");

                                while(true)
                                {
                                    bool isAmountNumber = decimal.TryParse(Console.ReadLine(),out decimal amount);

                                    if (!isAmountNumber)
                                    {
                                        Console.WriteLine("Ожидается число");
                                        continue;
                                    }

                                    foundUser.Deposite(amount);

                                    Console.WriteLine($"Баланс пополнен: {amount}");

                                    break;
                                }

                                isRunning = false;
                                
                            }

                           
                            break;
                        }
                }
                
            }
        }
    }
}
