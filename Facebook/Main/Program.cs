using Main.Menus;
using Main.Services;

namespace Main
{
    internal class Program
    {
        public static void Register()
        {
            UserService userService = new UserService();
            Console.WriteLine("============================");
            Console.WriteLine("Register Menu");
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            
            do
            {
                try
                {
                    Console.WriteLine("============================");
                    Console.Write("Enter your Username: ");
                    string name = Console.ReadLine();
                    if (Validation.Exit(name))
                        return;
                    Console.Write("Enter your Email: ");
                    string email = Console.ReadLine();
                    if (Validation.Exit(email))
                        return;
                    Console.Write("Enter your Password (Must be atleast 8 characters long, include an upper case, lower case, and a digit): ");
                    string password = Console.ReadLine();
                    if(Validation.Exit(password))
                        return;
                    Console.Write("Enter your date of birth (dd/ww/yyyy): ");
                    string dob = Console.ReadLine();
                    if (Validation.Exit(dob))
                        return;
                    Console.Write("Enter your Bio: ");
                    string bio = Console.ReadLine();
                    if (Validation.Exit(bio))
                        return;
                    
                    userService.AddUser(name, email, password, bio, dob);
                    Console.WriteLine("============================");
                    Console.WriteLine("User registered successfully!");
                    Console.WriteLine("============================");
                    return;
                } catch(ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
               
            }while (true);

        }

        public static void Login()
        {

            UserService userService = new UserService();
            Console.WriteLine("============================");
            Console.WriteLine("Login Menu");
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");

            do
            {
                try
                {
                    Console.WriteLine("============================");
                    Console.Write("Enter your Email: ");
                    string email = Console.ReadLine();
                    if (Validation.Exit(email))
                        return;
                    Console.Write("Enter your Password: ");
                    string password = Console.ReadLine();
                    if (Validation.Exit(password))
                        return;
                    int id = userService.Login(email, password);
                    StartMenu menu = new StartMenu(id);
                    Console.WriteLine("============================");
                    Console.WriteLine($"Login successful!\n-> Welcome {userService.GetUsername(id)}");
                    Console.WriteLine("============================");
                    menu.MainMenu();
                    return;

                }
                catch(ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (true);


        }

        static void Main(string[] args)
        {
            Console.WriteLine("\t\t\t===================Welcome to Facebook===================");
            Console.WriteLine("");
            Console.WriteLine("");
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.WriteLine("============================");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

            }while (true);

        }
    }
}
