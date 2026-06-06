using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace BankSystem_4._0
{
    class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<HistoryOfOperation> OperationHistory { get; set; } = new List<HistoryOfOperation>();
        public BankAccount(string name, int id)
        {
            Name = name;
            Id = id;
        }
        public BankAccount() { }
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
                AmountOfOperation = amount,
                Date = DateTime.Now
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
                AmountOfOperation = amount,
                Date = DateTime.Now
            });
        }
        public void ShowHistory()
        {
            foreach (var operation in OperationHistory)
            {
                Console.WriteLine(operation);
            }
        }
    }
    class Bank
    {
        private DataBase database;
        public Bank ()
        {
            database = new DataBase ();
        }
        public void CreateAccount (string name, int id)
        {
            database.SaveAccount(id, name);

            //SaveAccounts ();
        }
        public void ShowInfo ()
        {
            List<BankAccount> bankAccounts = database.GetBankAccounts ();

            foreach ( var account in bankAccounts )
            {
                Console.WriteLine(account);
            }
        }
        public BankAccount FindAccount (int id)
        {
            return database.GetAccount (id);
        }
        public bool CheckIsUniq (int id)
        {
            return database.GetAccount(id) != null;
        }
        public void UpdateAccount (BankAccount account)
        {
            database.UpdataBalance(account.Balance, account.Id);
        }
        public void SaveOperation (int accountid, OperationType type, decimal amount)
        {
            database.SaveAccountOperation(accountid, type, amount, DateTime.Now);
        }
        public List<HistoryOfOperation> GetHistore (int accountId)
        {
            return database.GetHistoreOfOperations(accountId);
        }
    }
    class HistoryOfOperation
    {
        public OperationType TypeOfOperation { get; set; }
        public decimal AmountOfOperation { get; set; }
        public DateTime Date {  get; set; }
        public override string ToString()
        {
            return $"{TypeOfOperation}: {AmountOfOperation}| {Date:f}";
        }
    }
    class DataBase
    {
        private SqliteConnection connection;
        public DataBase ()
        {
            connection = new SqliteConnection ("Data Source=BankDataBase.db");
            connection.Open ();
        }
        public void CreateTable()
        {
            string createTableSql =
                """
                    CREATE TABLE IF NOT EXISTS Accounts
                        (
                            Id INTEGER PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Balance REAL NOT NULL
                        )
                """;

            using var command = new SqliteCommand (createTableSql, connection);
            command.ExecuteNonQuery ();
        }
        public void CreateOperationTable ()
        {
            string createOperationTableSql =
                """
                    CREATE TABLE IF NOT EXISTS Operations
                    (
                        NumberOfOperation INTEGER PRIMARY KEY AUTOINCREMENT,
                        AccountId INTEGER NOT NULL,
                        OperationType TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        Date TEXT NOT NULL
                    )
                """;

            using var command = new SqliteCommand (createOperationTableSql, connection);
            command.ExecuteNonQuery ();
        }
        public void SaveAccount(int id, string name, decimal balance = 0)
        {
            string createAccountSql =
                """
                    INSERT INTO Accounts
                    (Id, Name, Balance)
                    VALUES
                    (@id, @name, @balance)
                """;

            using var command = new SqliteCommand (createAccountSql, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@balance", balance);
            command.ExecuteNonQuery();
        }
        public void SaveAccountOperation (int accountId, OperationType type, decimal amount, DateTime date)
        {
            string saveOperationSql =
                """
                    INSERT INTO Operations
                    (AccountId, OperationType, Amount, Date)
                    VALUES
                    (@accountid, @operationtype, @amount, @date)
                """;

            using var command = new SqliteCommand (saveOperationSql, connection);
            command.Parameters.AddWithValue("@accountid", accountId);
            command.Parameters.AddWithValue("@operationtype", type.ToString());
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd HH:mm"));
            command.ExecuteNonQuery();
        }
        public List<BankAccount> GetBankAccounts ()
        {
            List<BankAccount> accountsFromDB = new List<BankAccount> ();

            string selectAccountSql =
                """
                    SELECT * FROM Accounts
                """;

            using var command = new SqliteCommand (selectAccountSql, connection);

            var reader = command.ExecuteReader ();

            while (reader.Read())
            {
                accountsFromDB.Add(new BankAccount()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Name"]),
                    Balance = Convert.ToDecimal(reader["Balance"])
                });
            }

            return accountsFromDB;
        }
        public List<HistoryOfOperation> GetHistoreOfOperations (int accountId)
        {
            List<HistoryOfOperation> operations = new List<HistoryOfOperation> ();

            string selectOperationsSql =
                """
                    SELECT * FROM Operations
                    WHERE AccountId = @accountId
                """;

            using var command = new SqliteCommand ( selectOperationsSql, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            var reader = command.ExecuteReader ();
            while (reader.Read())
            {
                operations.Add(new HistoryOfOperation()
                {
                    TypeOfOperation = Enum.Parse<OperationType>(reader["OperationType"].ToString()),
                    AmountOfOperation = Convert.ToDecimal(reader["Amount"]),
                    Date = Convert.ToDateTime(reader["Date"])
                });
            }

            return operations;
        } 
        public BankAccount GetAccount(int id)
        {
            string selectAccountSql =
                 """
                    SELECT * FROM Accounts
                    WHERE Id = @id
                 """;

            using var command = new SqliteCommand (selectAccountSql, connection);
            command.Parameters.AddWithValue("@id", id);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new BankAccount()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Name"]),
                    Balance = Convert.ToDecimal(reader["Balance"])
                };
            }

            return null;
        }
        public void UpdataBalance (decimal balance, int id)
        {
            string updateBalanceSql =
                """
                    UPDATE Accounts
                    SET Balance = @balance
                    WHERE Id = @id
                """;

            using var command = new SqliteCommand (updateBalanceSql, connection);
            command.Parameters.AddWithValue("@balance", balance);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
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
                    bank.UpdateAccount(foundUser);
                    bank.SaveOperation(foundUser.Id, type, userAmount);

                    receiver.Deposit(userAmount);
                    bank.UpdateAccount(receiver);
                    bank.SaveOperation(receiver.Id, OperationType.Пополнение, userAmount);

                    Console.WriteLine($"Cписание: {userAmount}");
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
                bank.UpdateAccount(foundUser);
                bank.SaveOperation(foundUser.Id, type, userAmount);
                Console.WriteLine($"{type}: {userAmount}");
                //bank.SaveAccounts();

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
            //OperationType type = OperationType.Пополнение;

            Console.WriteLine("Пополнение баланса");

            var foundUser = GetAccount(bank);

            decimal userAmount = ReadAmount();

            try
            {
                foundUser.Deposit(userAmount);
                bank.UpdateAccount(foundUser);
                bank.SaveOperation(foundUser.Id, OperationType.Пополнение, userAmount);
                Console.WriteLine($"Баланс пополнен: {userAmount}");

                //bank.SaveAccounts();
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

            foundUser.OperationHistory = bank.GetHistore(foundUser.Id);

            foundUser.ShowHistory();

            Console.WriteLine("\nНажмите на Enter...");

            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            Bank someBank = new Bank ();
            DataBase dataBase = new DataBase();
            dataBase.CreateTable();
            dataBase.CreateOperationTable();
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
