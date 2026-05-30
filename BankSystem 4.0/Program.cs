using System.Text.Json;

namespace BankSystem_4._0
{
    class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public List<HistoryOfOperation> OperationHistory { get; set; } = new List<HistoryOfOperation>();

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
        public List <BankAccount> accounts = new List <BankAccount> ();

        public void CreateAccount (string name, int id)
        {
            accounts.Add(new BankAccount
            {
                Id = id,
                Name = name,
            });

            SaveAccounts ();
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

        public void SaveAccounts()
        {
            string jsonAccounts = JsonSerializer.Serialize(accounts, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText("savedAccounts.json", jsonAccounts);
        }

        public List<BankAccount> LoadAccounts ()
        {
            string jsonAccountsFromFile = File.ReadAllText("savedAccounts.json");
            return JsonSerializer.Deserialize<List<BankAccount>>(jsonAccountsFromFile);
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
        static void HandleTransferMoney (Bank bank)
        {
            while (true)
            {
                OperationType type = OperationType.Перевод;

                Console.WriteLine("Перевод денег по ID");

                var foundUser = GetAccount(bank);

                Console.WriteLine("ID получателя");

                var receiver = GetAccount(bank);

                if (foundUser.Id == receiver.Id)
                {
                    Console.WriteLine("Нельзя перевести своему же аккаунту");
                    continue;
                }

                decimal userAmount = ReadAmount();

                try
                {
                    foundUser.Withdraw(userAmount, type);

                    receiver.Deposit(userAmount);

                    Console.WriteLine($"Cписание: {userAmount}");

                    bank.SaveAccounts();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine("\nНажмите на Enter...");

                Console.ReadLine();

                break;

            }

        }
        static void HandleWithdrawMoney(Bank bank)
        {
            OperationType type = OperationType.Списание;

            Console.WriteLine("Cписание средств");

            var foundUser = GetAccount(bank);

            decimal userAmount = ReadAmount();

            try
            {
                foundUser.Withdraw(userAmount, type);
                Console.WriteLine($"{type}: {userAmount}");
                bank.SaveAccounts();

            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Нажмите на Enter...");

            Console.ReadLine();
        }
        static void HandleDepositMoney (Bank bank)
        {
            Console.WriteLine("Пополнение баланса");

            var foundUser = GetAccount(bank);

            decimal userAmount = ReadAmount();

            try
            {
                foundUser.Deposit(userAmount);

                Console.WriteLine($"Баланс пополнен: {userAmount}");

                bank.SaveAccounts();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Нажмите на Enter...");
            Console.ReadLine();
        }
        static void HandleShowAccounts (Bank bank)
        {
            bank.ShowInfo();

            Console.WriteLine("Нажмите на Enter...");

            Console.ReadLine();
        }
        static void HandleOperationHistory (Bank bank)
        {
            Console.WriteLine("Посмотреть историю операций");

            var foundUser = GetAccount(bank);

            foundUser.ShowHistory();

            Console.WriteLine("\nНажмите на Enter...");

            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            if (File.Exists("savedAccounts.json"))
            {
                someBank.accounts = someBank.LoadAccounts();
            }
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
                            HandleShowAccounts(someBank);
                            break;
                        }
                    case 3:
                        {
                            HandleDepositMoney(someBank);
                            break;
                        }
                    case 4:
                        {
                            HandleWithdrawMoney(someBank);
                            break;
                        }
                    case 5:
                        {
                            HandleOperationHistory(someBank);
                            break;
                        }
                    case 6:
                        {
                            HandleTransferMoney(someBank);
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
