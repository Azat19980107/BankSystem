namespace BankSystem_4._0
{
    class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        List<HistoryOfOperation> OperationHistory = new List<HistoryOfOperation>();
        public override string ToString()
        {
            return $"{Name}, {Id}, Баланс: {Balance}";
        }
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Сумма пополнения не может быть меньше или равно 0");
            }

            Balance += amount;

            OperationHistory.Add(new HistoryOfOperation
            {
                TypeOfOperation = "Пополнение",
                AmountOfOperation = amount
            });
        }
        public void Withdraw (decimal amount, string type)
        {
            if (amount > Balance)
            {
                throw new ArgumentException("Недостаточно средств");
            }
            else if (amount <= 0)
            {
                throw new ArgumentException("Сумма не может быть меньше или равно 0");
            }

            Balance -= amount;

            OperationHistory.Add(new HistoryOfOperation
            {
                TypeOfOperation = type,
                AmountOfOperation = amount
            });
        }
        public void ShowHistory ()
        {
            foreach ( var operation in OperationHistory )
            {
                Console.WriteLine(operation);
            }
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

    class HistoryOfOperation
    {
        public string TypeOfOperation { get; set; }
        public decimal AmountOfOperation { get; set; }

        public override string ToString()
        {
            return $"{TypeOfOperation}: {AmountOfOperation}";
        }
    }
    internal class Program
    {
        static int ReadId ()
        {
            Console.WriteLine("Введите ID");

            while (true)
            {
                bool isNumber = int.TryParse(Console.ReadLine(), out int userId);

                if (!isNumber)
                {
                    Console.WriteLine("ID должен быть числовым, попробуйте еще раз");
                    continue;
                }

                return userId;
            }
        }
        static decimal ReadAmount()
        {
            Console.WriteLine("Введите сумму");

            while (true)
            {
                bool isNumber = decimal.TryParse(Console.ReadLine(),out decimal userAmount);

                if (!isNumber)
                {
                    Console.WriteLine("Сумма должна быть числом");
                    continue;
                }

                return userAmount;
            }
        }
        static BankAccount GetAccount (Bank bank)
        {
            while (true)
            {
                var foundAccount = bank.FindAccount(ReadId());

                if (foundAccount == null)
                {
                    Console.WriteLine("Аккаунт не найден, попробуйте еще раз");
                    continue;
                }

                return foundAccount;
            }

        }
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            BankAccount account = new BankAccount ();
            
            while (true)
            {
                Console.Clear ();
                Console.WriteLine(
                    "1 - Создать аккаунт\n" +
                    "2 - Посмотреть аккаунты\n" +
                    "3 - Пополнить баланс\n" +
                    "4 - Cнять средства\n" +
                    "5 - История операций аккаунта\n" +
                    "6 - Перевод средств");

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
                            bool isRunning = true;
                            while (isRunning)
                            {
                                string name = Console.ReadLine();

                                if (name.Length < 3)
                                {
                                    Console.WriteLine("Имя должно быть из не менее трех букв");
                                    continue;
                                }

                                if(string.IsNullOrWhiteSpace(name))
                                {
                                    Console.WriteLine("Имя не может пустым или содержать пробелы");
                                    continue;
                                }

                                bool doesContainDigit = false;

                                foreach (char letter in name)
                                {
                                    if (char.IsDigit(letter))
                                    {
                                        doesContainDigit = true;
                                    }
                                
                                }

                                if (doesContainDigit)
                                {
                                    Console.WriteLine("Имя содержит цифру, попробуйте еще раз");
                                    continue;
                                }

                                bool isSymbol = false;

                                foreach (char letter in name)
                                {
                                    if (!char.IsLetter(letter))
                                    {
                                        isSymbol = true;
                                    }
                                }

                                if (isSymbol)
                                {
                                    Console.WriteLine("Имя содержит символы, попробуйте еще раз");
                                    continue;
                                }

                                someBank.CreateAccount(name, ReadId());

                                Console.WriteLine("Аккаунт создан. Нажмите Enter...");

                                Console.ReadLine();

                                isRunning = false;
                            }

                            break;
                        }
                    case 2:
                        {
                            someBank.ShowInfo();

                            Console.WriteLine("Нажмите на Enter...");

                            Console.ReadLine();

                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Пополнение баланса");

                            var foundUser = GetAccount(someBank);

                            decimal userAmount = ReadAmount();

                            try
                            {
                                foundUser.Deposit(userAmount);

                                Console.WriteLine($"Баланс пополнен: {userAmount}");
                            }
                            catch(ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            Console.WriteLine("Нажмите на Enter...");
                            Console.ReadLine();

                            break;
                        }
                    case 4:
                        {
                            string type = "Списание";

                            Console.WriteLine("Cписание средств");

                            var foundUser = GetAccount(someBank);

                            decimal userAmount = ReadAmount();

                            try
                            {
                                foundUser.Withdraw(userAmount, type);
                                Console.WriteLine($"{type}: {userAmount}");

                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            Console.WriteLine("Нажмите на Enter...");

                            Console.ReadLine ();

                            
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("Посмотреть историю операций");

                            var foundUser = GetAccount(someBank);

                            foundUser.ShowHistory();

                            Console.WriteLine("\nНажмите на Enter...");

                            Console.ReadLine();
                            
                            break;
                        }
                    case 6:
                        {
                            string type = "Перевод средств";

                            Console.WriteLine("Перевод денег по ID");

                            var foundUser = GetAccount(someBank);

                            Console.WriteLine("ID получателя");

                            var receiver = GetAccount(someBank);

                            decimal userAmount = ReadAmount();

                            foundUser.Withdraw(userAmount, type);

                            receiver.Deposit(userAmount);

                            Console.WriteLine($"Cписание: {userAmount}");

                            Console.WriteLine("\nНажмите на Enter...");

                            Console.ReadLine();

                            break;
                        }
                }
                
            }
        }
    }
}
