using System.Text.Json;

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
                TypeOfOperation = OperationType.Пополнение,
                AmountOfOperation = amount
            });
        }
        public void Withdraw (decimal amount, OperationType type)
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

            string json = JsonSerializer.Serialize(accounts);
            File.WriteAllText("accounts.json", json);
        }

        public void LoadAccounts ()
        {
            if (File.Exists("accounts.json"))
            {
                string jsonFromFile = File.ReadAllText("accounts.json");
                accounts = JsonSerializer.Deserialize<List<BankAccount>>(jsonFromFile);
            }
            else
            {
                Console.WriteLine("Файл не найден");
            }
            
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

        public bool CheckIsUniq (int id)
        {
            return accounts.Any(account => account.Id == id);
        }
        
    }

    class HistoryOfOperation
    {
        public OperationType TypeOfOperation { get; set; }
        public decimal AmountOfOperation { get; set; }

        public override string ToString()
        {
            return $"{TypeOfOperation}: {AmountOfOperation}";
        }
    }

    enum OperationType
    {
        Пополнение,
        Списание,
        Перевод
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
        static string ReadName ()
        {
            while(true)
            {
                Console.WriteLine("Введите имя");

                string name = Console.ReadLine();

                if (CheckSpaceis(name))
                {
                    Console.WriteLine("Имя не может пустым или содержать пробелы");
                    continue;
                }

                if (!CheckLengthOfName(name))
                {
                    Console.WriteLine("Имя должно быть из не менее трех букв");
                    continue;
                }

                if (DoesContainDigitOrSymbols(name))
                {
                    Console.WriteLine("Имя содержит цифры или символы");
                    continue;
                }

                return name;

            }
        }

        static bool CheckLengthOfName (string name)
        {
            bool isMoreThenThreeSymbols = true;

            if (name.Length < 3)
            {
                isMoreThenThreeSymbols = false; 
            }

            return isMoreThenThreeSymbols;
        }
        static bool CheckSpaceis (string name)
        {
            bool doesContainSpacesisOrEmptyness = false;

            if (string.IsNullOrWhiteSpace(name))
            {
                doesContainSpacesisOrEmptyness = true;
            }

            return doesContainSpacesisOrEmptyness;
        }

        static bool DoesContainDigitOrSymbols (string name)
        {
            bool doesContain = false;

            foreach (char letter in name)
            {
                if (!char.IsLetter(letter))
                {
                    doesContain = true;
                }
            }

            return doesContain;
        }

        static void HandleCreateAccount (Bank bank)
        {
            string name = ReadName();

            while (true)
            {
                int id = ReadId();

                if(bank.CheckIsUniq(id))
                {
                    Console.WriteLine("ID уже существует");
                    continue;
                }

                bank.CreateAccount(name, id);
                Console.WriteLine("Аккаунт создан. Нажмите Enter...");
                Console.ReadLine();
                break;
            }


        }
        static bool CheckIsExist (int id, Bank bank)
        {
            bool isExist = bank.CheckIsUniq(id);
            return isExist;
        }
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            someBank.LoadAccounts();
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
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
                            HandleCreateAccount(someBank);

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
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            Console.WriteLine("Нажмите на Enter...");
                            Console.ReadLine();

                            break;
                        }
                    case 4:
                        {
                            OperationType type = OperationType.Списание;

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

                            Console.ReadLine();


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
                            OperationType type = OperationType.Перевод;

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
                    case 0:
                        {
                            Console.WriteLine("Завершение программы, нажмите Enter...");
                            Console.ReadLine();
                            return;
                        }
                }

            }
        }
    }
}
