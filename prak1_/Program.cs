using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract class Person
{
    public string Name { get; private set; }

    protected Person(string name)
    {
        Name = name;
    }

    public abstract void ShowInfo();
}

public class User : Person
{
    public List<Book> BorrowedBooks { get; private set; }

    public User(string name) : base(name)
    {
        BorrowedBooks = new List<Book>();
    }

    public void BorrowBook(Book book)
    {
        if (book.IsAvailable)
        {
            book.Borrow();
            BorrowedBooks.Add(book);
            Console.WriteLine($"Книга '{book.Title}' успешно взята.");
        }
        else
        {
            Console.WriteLine($"Книга '{book.Title}' уже выдана.");
        }
    }

    public void ReturnBook(Book book)
    {
        if (BorrowedBooks.Remove(book))
        {
            book.Return();
            Console.WriteLine($"Книга '{book.Title}' успешно возвращена.");
        }
        else
        {
            Console.WriteLine($"Книга '{book.Title}' не найдена в ваших взятых.");
        }
    }

    public void ListBorrowedBooks()
    {
        Console.WriteLine($"Список взятых книг для пользователя '{Name}':");
        if (!BorrowedBooks.Any())
        {
            Console.WriteLine("Нет взятых книг.");
        }
        else
        {
            foreach (var book in BorrowedBooks)
            {
                Console.WriteLine(book);
            }
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine($"Пользователь: {Name}");
    }
}

public class Librarian : Person
{
    public Librarian(string name) : base(name)
    {
    }

    public void AddBook(List<Book> books, string title, string author)
    {
        books.Add(new Book(title, author));
        Console.WriteLine($"Книга '{title}' добавлена.");
    }

    public void RemoveBook(List<Book> books, string title)
    {
        var book = books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null)
        {
            books.Remove(book);
            Console.WriteLine($"Книга '{title}' удалена.");
        }
        else
        {
            Console.WriteLine($"Книга '{title}' не найдена.");
        }
    }

    public void RegisterUser(List<User> users, string userName)
    {
        if (users.Any(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Пользователь '{userName}' уже зарегистрирован.");
            return;
        }

        users.Add(new User(userName));
        Console.WriteLine($"Пользователь '{userName}' зарегистрирован.");
    }

    public void ListAllUsers(List<User> users)
    {
        Console.WriteLine("Список всех пользователей:");
        foreach (var user in users)
        {
            user.ShowInfo();
            user.ListBorrowedBooks();
        }
    }

    public void ListAllBooks(List<Book> books)
    {
        Console.WriteLine("Список всех книг:");
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {books[i]}");
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine($"Библиотекарь: {Name}");
    }
}

public class Book
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public bool IsAvailable { get; private set; }

    public Book(string title, string author)
    {
        Title = title;
        Author = author;
        IsAvailable = true;
    }

    public void Borrow() => IsAvailable = false;
    public void Return() => IsAvailable = true;

    public override string ToString() => $"{Title}, {Author}, {(IsAvailable ? "Доступна" : "Выдана")}";
}

public class LibrarySystem
{
    private List<Book> books;
    private List<User> users;
    private Librarian librarian;
    private const string booksFilePath = "D:\\MPT\\C# ОАиП\\Проекты\\PRAK1_\\books.txt";
    private const string usersFilePath = "D:\\MPT\\C# ОАиП\\Проекты\\PRAK1_\\user_books.txt";

    public LibrarySystem(Librarian librarian)
    {
        this.librarian = librarian;
        books = new List<Book>();
        users = new List<User>();
        LoadData();
    }

    public User UserLogin()
    {
        Console.Write("Введите ваше имя: ");
        var userName = Console.ReadLine();
        var user = users.Find(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            Console.WriteLine("Пользователь не зарегистрирован. Обратитесь к библиотекарю.");
            return null;
        }

        return user;
    }

    public void UserMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("1. Просмотреть доступные книги");
            Console.WriteLine("2. Взять книгу");
            Console.WriteLine("3. Вернуть книгу");
            Console.WriteLine("4. Просмотреть список взятых книг");
            Console.WriteLine("0. Выход");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    librarian.ListAllBooks(books);
                    break;
                case "2":
                    Console.Write("Введите номер книги для взятия: ");
                    if (int.TryParse(Console.ReadLine(), out int borrowIndex) && borrowIndex > 0 && borrowIndex <= books.Count)
                    {
                        user.BorrowBook(books[borrowIndex - 1]);
                        SaveAllData();
                    }
                    else
                    {
                        Console.WriteLine("Некорректный номер книги.");
                    }
                    break;
                case "3":
                    Console.Write("Введите номер книги для возврата: ");
                    if (int.TryParse(Console.ReadLine(), out int returnIndex) && returnIndex > 0 && returnIndex <= books.Count)
                    {
                        user.ReturnBook(books[returnIndex - 1]);
                        SaveAllData();
                    }
                    else
                    {
                        Console.WriteLine("Некорректный номер книги.");
                    }
                    break;
                case "4":
                    user.ListBorrowedBooks();
                    break;
                case "0":
                    return;
            }
        }
    }

    public void LibrarianMenu()
    {
        while (true)
        {
            Console.WriteLine("1. Добавить новую книгу");
            Console.WriteLine("2. Удалить книгу");
            Console.WriteLine("3. Зарегистрировать нового пользователя");
            Console.WriteLine("4. Просмотреть список всех пользователей");
            Console.WriteLine("5. Просмотреть список всех книг");
            Console.WriteLine("0. Выход");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите название книги: ");
                    var title = Console.ReadLine();
                    Console.Write("Введите автора книги: ");
                    var author = Console.ReadLine();
                    librarian.AddBook(books, title, author);
                    SaveAllData();
                    break;
                case "2":
                    Console.Write("Введите название книги для удаления: ");
                    var removeTitle = Console.ReadLine();
                    librarian.RemoveBook(books, removeTitle);
                    SaveAllData();
                    break;
                case "3":
                    Console.Write("Введите имя нового пользователя: ");
                    var userName = Console.ReadLine();
                    librarian.RegisterUser(users, userName);
                    SaveUsersBooksToFile(usersFilePath);
                    break;
                case "4":
                    librarian.ListAllUsers(users);
                    break;
                case "5":
                    librarian.ListAllBooks(books);
                    break;
                case "0":
                    return;
            }
        }
    }

    private void LoadData()
    {
        LoadBooksFromFile(booksFilePath);
        LoadUsersBooksFromFile(usersFilePath);
    }

    private void LoadBooksFromFile(string filename)
    {
        if (File.Exists(filename))
        {
            var lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 3)
                {
                    var book = new Book(parts[0].Trim(), parts[1].Trim());
                    if (parts[2].Trim() == "Выдана") book.Borrow();
                    books.Add(book);
                }
            }
        }
    }

    private void SaveBooksToFile(string filename)
    {
        var lines = new List<string>();
        foreach (var book in books)
        {
            lines.Add($"{book.Title},{book.Author},{(book.IsAvailable ? "Доступна" : "Выдана")}");
        }
        File.WriteAllLines(filename, lines);
    }

    private void LoadUsersBooksFromFile(string filename)
    {
        if (File.Exists(filename))
        {
            var lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var userName = parts[0].Trim();
                    var user = users.Find(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        user = new User(userName);
                        users.Add(user);
                    }
                    var borrowedTitles = parts[1].Split(',').Select(t => t.Trim());

                    foreach (var title in borrowedTitles)
                    {
                        var book = books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
                        if (book != null)
                        {
                            user.BorrowedBooks.Add(book);
                        }
                    }
                }
            }
        }
    }

    private void SaveUsersBooksToFile(string filename)
    {
        var lines = new List<string>();
        foreach (var user in users)
        {
            var borrowedBooks = string.Join(',', user.BorrowedBooks.Select(b => b.Title));
            lines.Add($"{user.Name}:{borrowedBooks}");
        }
        File.WriteAllLines(filename, lines);
    }

    private void SaveAllData()
    {
        SaveBooksToFile(booksFilePath);
        SaveUsersBooksToFile(usersFilePath);
    }

    public void Exit()
    {
        SaveAllData();
    }
}

class Program
{
    static void Main(string[] args)
    {
        LibrarySystem librarySystem = new LibrarySystem(new Librarian("Библиотекарь"));

        Console.WriteLine("Выберите роль: 1. Библиотекарь 2. Пользователь");
        var roleChoice = Console.ReadLine();

        if (roleChoice == "1")
        {
            librarySystem.LibrarianMenu();
        }
        else if (roleChoice == "2")
        {
            var user = librarySystem.UserLogin();
            if (user != null)
            {
                librarySystem.UserMenu(user);
            }
        }

        librarySystem.Exit();
    }
}
