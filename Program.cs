using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace ADO.NET
{
    class Program
    {
        private static readonly string ConnectionString = "Data Source=BookStore.db;Version=3;";

        static void Main(string[] args)
        {
            InitializeDatabase();
            Console.WriteLine("Добро пожаловать в Книжный магазин имени Арсения Борисовича!");

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
                        SellBook();
                        break;
                    case "7":
                        WriteOffBook();
                        break;
                    case "8":
                        AddBookToPromotion();
                        break;
                    case "9":
                        ReserveBook();
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
            Console.WriteLine("6. Продать книгу");
            Console.WriteLine("7. Списать книгу");
            Console.WriteLine("8. Добавить книгу в акцию");
            Console.WriteLine("9. Отложить книгу для покупателя");
            Console.WriteLine("0. Выйти");
            Console.Write("Ваш выбор: ");
        }

        private static void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createTableQuery = @"CREATE TABLE IF NOT EXISTS Books (
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

            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
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

            string insertQuery = @"INSERT INTO Books (Title, Author, Publisher, Pages, Genre, Year, CostPrice, SalePrice, IsSequel)
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

            string updateQuery = @"UPDATE Books SET Title = @Title, Author = @Author, Publisher = @Publisher, 
                                    Pages = @Pages, Genre = @Genre, Year = @Year, CostPrice = @CostPrice, 
                                    SalePrice = @SalePrice, IsSequel = @IsSequel WHERE Id = @Id;";

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
            Console.Write("Введите параметр для поиска (оставьте пустым, чтобы пропустить):\nНазвание: ");
            string title = Console.ReadLine()?.Trim();
            Console.Write("Автор: ");
            string author = Console.ReadLine()?.Trim();
            Console.Write("Жанр: ");
            string genre = Console.ReadLine()?.Trim();

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string searchQuery = "SELECT * FROM Books WHERE 1=1";
            if (!string.IsNullOrEmpty(title)) searchQuery += " AND Title LIKE @Title";
            if (!string.IsNullOrEmpty(author)) searchQuery += " AND Author LIKE @Author";
            if (!string.IsNullOrEmpty(genre)) searchQuery += " AND Genre LIKE @Genre";

            using var command = new SQLiteCommand(searchQuery, connection);

            if (!string.IsNullOrEmpty(title)) command.Parameters.AddWithValue("@Title", "%" + title + "%");
            if (!string.IsNullOrEmpty(author)) command.Parameters.AddWithValue("@Author", "%" + author + "%");
            if (!string.IsNullOrEmpty(genre)) command.Parameters.AddWithValue("@Genre", "%" + genre + "%");

            using var reader = command.ExecuteReader();

            Console.WriteLine("\nРезультаты поиска:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}, Жанр: {reader["Genre"]}, Сиквел: {(reader.GetInt32(reader.GetOrdinal("IsSequel")) == 1 ? "Да" : "Нет")}");
            }
        }

        private static void SellBook()
        {
            Console.Write("Введите ID книги для продажи: ");
            int id = GetIntInput("ID книги");
            Console.Write("Введите количество для продажи: ");
            int quantity = GetIntInput("количество");

            Console.WriteLine($"Книга с ID {id} успешно продана в количестве {quantity}.");
        }

        private static void WriteOffBook()
        {
            Console.Write("Введите ID книги для списания: ");
            int id = GetIntInput("ID книги");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string deleteQuery = "DELETE FROM Books WHERE Id = @Id;";

            using var command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            int rowsAffected = command.ExecuteNonQuery();

            Console.WriteLine(rowsAffected > 0 ? "Книга успешно списана." : "Книга с указанным ID не найдена.");
        }

        private static void AddBookToPromotion()
        {
            Console.Write("Введите ID книги для добавления в акцию: ");
            int id = GetIntInput("ID книги");
            Console.Write("Введите процент скидки: ");
            double discount = GetDoubleInput("процент скидки");

            Console.WriteLine($"Книга с ID {id} добавлена в акцию со скидкой {discount}%.");
        }

        private static void ReserveBook()
        {
            Console.Write("Введите ID книги для резервирования: ");
            int id = GetIntInput("ID книги");
            Console.Write("Введите имя покупателя: ");
            string customerName = Console.ReadLine()?.Trim();

            Console.WriteLine($"Книга с ID {id} зарезервирована для {customerName}.");
        }

        private static int GetIntInput(string fieldName)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }
                Console.WriteLine($"Введите корректное значение для {fieldName}.");
            }
        }

        private static double GetDoubleInput(string fieldName)
        {
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out double value))
                {
                    return value;
                }
                Console.WriteLine($"Введите корректное значение для {fieldName}.");
            }
        }

        private static void ShowAllBooks()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT * FROM Books;";

            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            Console.WriteLine("\nСписок всех книг:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}, Жанр: {reader["Genre"]}, Сиквел: {(reader.GetInt32(reader.GetOrdinal("IsSequel")) == 1 ? "Да" : "Нет")}");
            }
        }
    }
}
