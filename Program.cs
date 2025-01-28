using System;
using System.Data.SQLite;

namespace ADO.NET
{
    class Program
    {
        private static readonly string ConnectionString = "Data Source=BookStore.db;Version=3;";

        static void Main(string[] args)
        {
            InitializeDatabase();
            Console.WriteLine("Добро пожаловать в Книжный магазин!");

            if (!AuthenticateUser())
            {
                Console.WriteLine("Ошибка входа. Завершение программы.");
                return;
            }

            while (true)
            {
                DisplayMenu();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        DeleteBook();
                        break;
                    case "3":
                        EditBook();
                        break;
                    case "4":
                        SearchBooks();
                        break;
                    case "5":
                        ShowAllBooks();
                        break;
                    case "6":
                        ShowNewBooks();
                        break;
                    case "7":
                        ShowTopSellingBooks();
                        break;
                    case "8":
                        ShowPopularAuthors();
                        break;
                    case "9":
                        ShowPopularGenres();
                        break;
                    case "0":
                        Console.WriteLine("Выход из программы.");
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу");
            Console.WriteLine("3. Редактировать книгу");
            Console.WriteLine("4. Найти книгу");
            Console.WriteLine("5. Показать все книги");
            Console.WriteLine("6. Показать новинки");
            Console.WriteLine("7. Показать самые продаваемые книги");
            Console.WriteLine("8. Показать популярных авторов");
            Console.WriteLine("9. Показать популярные жанры");
            Console.WriteLine("0. Выйти");
            Console.Write("Ваш выбор: ");
        }

        private static void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createBooksTable = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Publisher TEXT,
                    Pages INTEGER,
                    Genre TEXT,
                    Year INTEGER,
                    CostPrice REAL,
                    SalePrice REAL,
                    IsSequel INTEGER DEFAULT 0
                );";

            string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                );";

            string createSalesTable = @"
                CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    SaleDate DATETIME NOT NULL,
                    FOREIGN KEY (BookId) REFERENCES Books(Id)
                );";

            using var commandBooks = new SQLiteCommand(createBooksTable, connection);
            commandBooks.ExecuteNonQuery();

            using var commandUsers = new SQLiteCommand(createUsersTable, connection);
            commandUsers.ExecuteNonQuery();

            using var commandSales = new SQLiteCommand(createSalesTable, connection);
            commandSales.ExecuteNonQuery();
        }

        private static bool AuthenticateUser()
        {
            Console.Write("Введите логин: ");
            string username = Console.ReadLine()?.Trim();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine()?.Trim();

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            int userExists = Convert.ToInt32(command.ExecuteScalar());
            return userExists > 0;
        }

        private static void AddBook()
        {
            Console.Write("Введите название книги: ");
            string title = Console.ReadLine()?.Trim();
            Console.Write("Введите автора книги: ");
            string author = Console.ReadLine()?.Trim();
            Console.Write("Введите издательство: ");
            string publisher = Console.ReadLine()?.Trim();
            Console.Write("Введите количество страниц: ");
            int pages = GetIntInput("количество страниц");
            Console.Write("Введите жанр: ");
            string genre = Console.ReadLine()?.Trim();
            Console.Write("Введите год издания: ");
            int year = GetIntInput("год издания");
            Console.Write("Введите себестоимость: ");
            double costPrice = GetDoubleInput("себестоимость");
            Console.Write("Введите цену для продажи: ");
            double salePrice = GetDoubleInput("цену для продажи");
            Console.Write("Является ли книга сиквелом (1 - Да, 0 - Нет): ");
            int isSequel = GetIntInput("сиквел");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string insertQuery = @"
                INSERT INTO Books (Title, Author, Publisher, Pages, Genre, Year, CostPrice, SalePrice, IsSequel)
                VALUES (@Title, @Author, @Publisher, @Pages, @Genre, @Year, @CostPrice, @SalePrice, @IsSequel);";

            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Author", author);
            command.Parameters.AddWithValue("@Publisher", publisher);
            command.Parameters.AddWithValue("@Pages", pages);
            command.Parameters.AddWithValue("@Genre", genre);
            command.Parameters.AddWithValue("@Year", year);
            command.Parameters.AddWithValue("@CostPrice", costPrice);
            command.Parameters.AddWithValue("@SalePrice", salePrice);
            command.Parameters.AddWithValue("@IsSequel", isSequel);

            command.ExecuteNonQuery();
            Console.WriteLine("Книга успешно добавлена.");
        }

        private static void DeleteBook()
        {
            Console.Write("Введите ID книги для удаления: ");
            int id = GetIntInput("ID книги");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string deleteQuery = "DELETE FROM Books WHERE Id = @Id;";
            using var command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Книга успешно удалена." : "Книга с указанным ID не найдена.");
        }

        private static void EditBook()
        {
            Console.Write("Введите ID книги для редактирования: ");
            int id = GetIntInput("ID книги");

            Console.Write("Введите новое название книги: ");
            string title = Console.ReadLine()?.Trim();
            Console.Write("Введите нового автора книги: ");
            string author = Console.ReadLine()?.Trim();
            Console.Write("Введите новое издательство: ");
            string publisher = Console.ReadLine()?.Trim();
            Console.Write("Введите новое количество страниц: ");
            int pages = GetIntInput("количество страниц");
            Console.Write("Введите новый жанр: ");
            string genre = Console.ReadLine()?.Trim();
            Console.Write("Введите новый год издания: ");
            int year = GetIntInput("год издания");
            Console.Write("Введите новую себестоимость: ");
            double costPrice = GetDoubleInput("себестоимость");
            Console.Write("Введите новую цену для продажи: ");
            double salePrice = GetDoubleInput("цену для продажи");
            Console.Write("Является ли книга сиквелом (1 - Да, 0 - Нет): ");
            int isSequel = GetIntInput("сиквел");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string updateQuery = @"
                UPDATE Books
                SET Title = @Title, Author = @Author, Publisher = @Publisher,
                    Pages = @Pages, Genre = @Genre, Year = @Year,
                    CostPrice = @CostPrice, SalePrice = @SalePrice, IsSequel = @IsSequel
                WHERE Id = @Id;";

            using var command = new SQLiteCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Author", author);
            command.Parameters.AddWithValue("@Publisher", publisher);
            command.Parameters.AddWithValue("@Pages", pages);
            command.Parameters.AddWithValue("@Genre", genre);
            command.Parameters.AddWithValue("@Year", year);
            command.Parameters.AddWithValue("@CostPrice", costPrice);
            command.Parameters.AddWithValue("@SalePrice", salePrice);
            command.Parameters.AddWithValue("@IsSequel", isSequel);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Книга успешно обновлена." : "Книга с указанным ID не найдена.");
        }

        private static void SearchBooks()
        {
            Console.Write("Введите параметры поиска (оставьте пустым для пропуска):");
            Console.Write("\nНазвание: ");
            string title = Console.ReadLine()?.Trim();
            Console.Write("Автор: ");
            string author = Console.ReadLine()?.Trim();
            Console.Write("Жанр: ");
            string genre = Console.ReadLine()?.Trim();

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT * FROM Books WHERE 1=1";
            if (!string.IsNullOrEmpty(title)) query += " AND Title LIKE @Title";
            if (!string.IsNullOrEmpty(author)) query += " AND Author LIKE @Author";
            if (!string.IsNullOrEmpty(genre)) query += " AND Genre LIKE @Genre";

            using var command = new SQLiteCommand(query, connection);
            if (!string.IsNullOrEmpty(title)) command.Parameters.AddWithValue("@Title", $"%{title}%");
            if (!string.IsNullOrEmpty(author)) command.Parameters.AddWithValue("@Author", $"%{author}%");
            if (!string.IsNullOrEmpty(genre)) command.Parameters.AddWithValue("@Genre", $"%{genre}%");

            using var reader = command.ExecuteReader();

            Console.WriteLine("\nРезультаты поиска:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}, Жанр: {reader["Genre"]}");
            }
        }

        private static void ShowAllBooks()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT * FROM Books;";
            using var command = new SQLiteCommand(query, connection);

            using var reader = command.ExecuteReader();
            Console.WriteLine("\nВсе книги:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}");
            }
        }

        private static void ShowNewBooks()
        {
            Console.Write("Введите год для фильтрации новинок: ");
            int year = GetIntInput("год");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT * FROM Books WHERE Year >= @Year;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Year", year);

            using var reader = command.ExecuteReader();
            Console.WriteLine("\nНовинки:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Год: {reader["Year"]}");
            }
        }

        private static void ShowTopSellingBooks()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = @"
                SELECT Books.Title, SUM(Sales.Quantity) AS Sold
                FROM Sales
                INNER JOIN Books ON Sales.BookId = Books.Id
                GROUP BY Sales.BookId
                ORDER BY Sold DESC
                LIMIT 10;";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            Console.WriteLine("\nСамые продаваемые книги:");
            while (reader.Read())
            {
                Console.WriteLine($"Название: {reader["Title"]}, Продано: {reader["Sold"]}");
            }
        }

        private static void ShowPopularAuthors()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = @"
                SELECT Books.Author, SUM(Sales.Quantity) AS Sold
                FROM Sales
                INNER JOIN Books ON Sales.BookId = Books.Id
                GROUP BY Books.Author
                ORDER BY Sold DESC
                LIMIT 10;";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            Console.WriteLine("\nПопулярные авторы:");
            while (reader.Read())
            {
                Console.WriteLine($"Автор: {reader["Author"]}, Продано: {reader["Sold"]}");
            }
        }

        private static void ShowPopularGenres()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = @"
                SELECT Books.Genre, SUM(Sales.Quantity) AS Sold
                FROM Sales
                INNER JOIN Books ON Sales.BookId = Books.Id
                GROUP BY Books.Genre
                ORDER BY Sold DESC
                LIMIT 10;";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            Console.WriteLine("\nПопулярные жанры:");
            while (reader.Read())
            {
                Console.WriteLine($"Жанр: {reader["Genre"]}, Продано: {reader["Sold"]}");
            }
        }

        private static int GetIntInput(string field)
        {
            while (true)
            {
                Console.Write($"Введите {field}: ");
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }
                Console.WriteLine("Некорректный ввод, попробуйте снова.");
            }
        }

        private static double GetDoubleInput(string field)
        {
            while (true)
            {
                Console.Write($"Введите {field}: ");
                if (double.TryParse(Console.ReadLine(), out double value))
                {
                    return value;
                }
                Console.WriteLine("Некорректный ввод, попробуйте снова.");
            }
        }
    }
}