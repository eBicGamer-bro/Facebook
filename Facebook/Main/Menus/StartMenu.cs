using Data.Entities;
using Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menus
{
    public class StartMenu : IMainMenu
    {
        private UserService _userService;
        private PostService _postService;
        private NotificationService _notificationService;
        private int _userId;
        public StartMenu(int userId)
        {
            _userService = new UserService();
            _postService = new PostService();
            _notificationService = new NotificationService();
            _userId = userId;
        }

        public void MainMenu()
        {
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("Main Menu:");
                Console.WriteLine("============================");
                Console.WriteLine("1. Posts Options");
                Console.WriteLine("2. Friends Options");
                Console.WriteLine("3. Groups Options");
                Console.WriteLine("4. Home Page");
                Console.WriteLine("5. Notifications");
                Console.WriteLine("6. Logout");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            PostsMenu postMenu = new PostsMenu(_userId);
                            postMenu.MainMenu();
                            break;
                        }
                        
                    case "2":
                        {
                            FriendsMenu friendsMenu = new FriendsMenu(_userId);
                            friendsMenu.MainMenu();
                            break;
                        }
                    case "3":
                        {
                            GroupsMenu groupsMenu = new GroupsMenu(_userId);
                            groupsMenu.MainMenu();
                            break;
                        }
                    case "4":
                        {
                            HomePage();
                            break;
                        }
                    case "5":
                        {
                            ViewNotifications();
                            break;
                        }
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }while (true);
        }

        public void HomePage()
        {
            PostsMenu postMenu = new PostsMenu(_userId);
            do
            {
                
                Console.WriteLine("============================");
                Console.WriteLine("Home Page:");
                Console.WriteLine("============================");
                Console.WriteLine("1. View Friends Posts");
                Console.WriteLine("2. View Group Posts");
                Console.WriteLine("3. View Latest Posts");
                Console.WriteLine("4. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            postMenu.ViewPosts(_postService.GetFriendsPosts(_userId));
                            break;
                        }

                    case "2":
                        {
                            postMenu.ViewPosts(_postService.GetGroupPosts(_userId));
                            break;
                        }
                    case "3":
                        {
                            postMenu.ViewPosts(_postService.GetOrderedPosts());
                            break;
                        }
                    case "4":
                        {
                            return;
                        }
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            } while (true);
        }

        public void ViewNotifications()
        {
            var notifications = _notificationService.GetNotifications(_userId);
            if (notifications.Count == 0)
            {
                Console.WriteLine("You have no new notifications!");
                return;
            }
            Console.WriteLine("============================");
            foreach (var notification in notifications)
            {
                Console.WriteLine(notification.Message);
            }
            
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. Mark All As Read");
                Console.WriteLine("2. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input1 = Console.ReadLine();
                switch(input1)
                {
                    case "1":
                        {
                            _notificationService.MarkAllAsRead(_userId);
                            Console.WriteLine("Notification Marked as Done Successfully");
                            return;
                        }
                    case "2":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid option"); 
                            break;
                        }
                }




            } while (true);

        }


    }
}
