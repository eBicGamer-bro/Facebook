using Data.Entities;
using Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menus
{
    public class FriendsMenu : IMainMenu
    {
        private FriendshipService friendshipService;
        private UserService userService;
        private int _userID;
        public FriendsMenu(int userId)
        {
            this.friendshipService = new FriendshipService();
            this.userService = new UserService();
            _userID = userId;
        }

        public void MainMenu()
        {
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("Friends Menu");
                Console.WriteLine("============================");
                Console.WriteLine("1. Send Friend Request");
                Console.WriteLine("2. View Friend Requests");
                Console.WriteLine("3. View Friend List");
                Console.WriteLine("4. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            SendFriendRequest();
                            break;
                        }
                    case "2":
                        {
                            ViewFriendRequests(); 
                            break;
                        }
                    case "3":
                        {
                            ViewFriends(); 
                            break;
                        }
                    case "4":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid Option!");
                            break;
                        }

                }
             }while (true);
        }

        public void SendFriendRequest()
        {
            var users = userService.GetUsers();

            foreach (var user in users)
            {
                if (user.UserID == _userID)
                    continue;
                Console.WriteLine($"{user.UserID}. {user.UserName}");
            }
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int userID;
            do
            {
                Console.Write("Select the user you want to add: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(users.Select(u => u.UserID).ToList(), input) || (userID = int.Parse(input)) == _userID)
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    break;
                }
            } while (true);

            try
            {
                friendshipService.SendFriendRequest(_userID, userID);
                Console.WriteLine("Friend Request Sent!");

            }catch (Exception ex)
            { 
                Console.WriteLine(ex.Message) ;
            }
        }

        public void ViewFriendRequests()
        {
            var friendRequests = friendshipService.GetPendingFriendRequests(_userID);
            if (friendRequests.Count == 0)
            {
                Console.WriteLine("You have no requests");
                return;
            }
            Console.WriteLine("============================");
            foreach(var request in friendRequests)
            {
                Console.WriteLine($"{request.FriendshipID}. {request.FriendName}");
            }

            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int friendShipID;
            do
            {
                Console.Write("Select the request you want to manage: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(friendRequests.Select(f => f.FriendshipID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    friendShipID = int.Parse(input);
                    break;
                }
            } while (true);

            do
            {
                try
                {
                    Console.WriteLine("============================");
                    Console.WriteLine("1. Accept Friend Request");
                    Console.WriteLine("2. Reject Friend Request");
                    Console.WriteLine("3. Back");
                    Console.WriteLine("============================");
                    Console.Write("Select an option: ");
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            {
                                friendshipService.AcceptFriendRequest(friendShipID);
                                Console.WriteLine("Friend Added Successfully");
                                return;
                            }
                        case "2":
                            {
                                friendshipService.RejectFriendRequest(friendShipID);
                                Console.WriteLine("Friend Rejected Successfully");
                                return;
                            }
                        case "3":
                            {
                                return;
                            }
                        default:
                            {
                                Console.WriteLine("Invalid Option!");
                                break;
                            }
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (true);

        }

        public void ViewFriends()
        {
            var friendsList = friendshipService.GetFriendsList(_userID);
            if(friendsList.Count == 0)
            {
                Console.WriteLine("You have no friends!");
                return;
            }
            Console.WriteLine("============================");
            foreach(var friend in friendsList)
            {
                Console.WriteLine($"{friend.FriendshipID}. {friend.FriendName}");
            }

            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int friendShipID;
            int friendID;
            do
            {
                Console.Write("Select the friend you want to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(friendsList.Select(f => f.FriendshipID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    friendShipID = int.Parse(input);
                    friendID = friendsList.First(f => f.FriendshipID == friendShipID).FriendID;
                    break;
                }
            } while (true);

            do
            {
                try
                {
                    Console.WriteLine("============================");
                    Console.WriteLine("1. View Friends Profile");
                    Console.WriteLine("2. View Common Friends");
                    Console.WriteLine("3. Remove Friend");
                    Console.WriteLine("4. Back");
                    Console.WriteLine("============================");
                    Console.Write("Select an option: ");
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            {
                                var profile = userService.GetFriendProfile(friendID);
                                Console.WriteLine($"Name: {profile.Name}");
                                Console.WriteLine($"Email: {profile.Email}");
                                Console.WriteLine($"Posts: ");
                                if (profile.Posts.Count == 0)
                                    Console.WriteLine("No posts found!");
                                else
                                foreach (var post in profile.Posts)
                                    Console.WriteLine(post);
                                break;
                            }
                        case "2":
                            {
                                ViewCommonFriends(friendID);
                                break;
                            }
                        case "3":
                            {
                                friendshipService.RemoveFriendship(friendShipID);
                                Console.WriteLine("Friend Removed Successfully");
                                return;
                            }
                        case "4":
                            {
                                return;
                            }
                        default:
                            {
                                Console.WriteLine("Invalid Option!");
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (true);
        }

        public void ViewCommonFriends(int friendId)
        {
            var friends = friendshipService.GetCommonFriends(_userID, friendId);
            if(friends.Count == 0)
            {
                Console.WriteLine("You have no common friends!");
                return;
            }
            Console.WriteLine("============================");
            foreach(var friend in friends)
            {
                Console.WriteLine($"{friend.UserID}. {friend.UserName}");
            }
            return;
        }
    }
}
