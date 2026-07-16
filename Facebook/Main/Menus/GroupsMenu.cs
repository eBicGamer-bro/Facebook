using Data.Entities;
using Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menus
{
    public class GroupsMenu : IMainMenu
    {
        private GroupService groupService;
        private PostService postService;
        private int _userId;

        public GroupsMenu(int userId)
        {
            groupService = new GroupService();
            postService = new PostService();
            _userId = userId;
        }

        public void MainMenu()
        {
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("Groups Menu");
                Console.WriteLine("============================");
                Console.WriteLine("1. View All Groups");
                Console.WriteLine("2. View Joined Groups");
                Console.WriteLine("3. Create Group");
                Console.WriteLine("4. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            ViewAllGroups();
                            break;
                        }
                    case "2":
                        {
                            ViewJoinedGroups();
                            break;
                        }
                    case "3":
                        {
                            CreateGroup();
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
            } while (true);
        }

        public void CreateGroup()
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            do
            {
                try
                {
                    Console.Write("Enter your group name: ");
                    string input = Console.ReadLine();
                    if (Validation.Exit(input))
                    {
                        return;
                    }
                    groupService.CreateGroup(_userId, input);
                    Console.WriteLine("Group Created Successfully");
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (true);

        }

        public void ViewAllGroups()
        {
            var groups = groupService.GetAllGroups();
            if (groups.Count == 0)
            {
                Console.WriteLine("No group exists yet!");
                return;
            }
            Console.WriteLine("============================");
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.GroupID}. {group.GroupName}");
            }

            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int groupId;
            do
            {
                Console.Write("Select a Group to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(groups.Select(p => p.GroupID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    groupId = int.Parse(input);
                    break;
                }
            } while (true);

            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. Join Group");
                Console.WriteLine("2. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            try
                            {
                                groupService.JoinGroup(_userId, groupId);
                                Console.WriteLine("Joined Group Successfully");
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                        }
                    case "2":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid Option!");
                            return;
                        }


                }


            } while (true);


        }

        public void ViewJoinedGroups()
        {
            var groups = groupService.GetUsersGroups(_userId);
            if (groups.Count == 0)
            {
                Console.WriteLine("You have not joined/created any groups yet!");
                return;
            }
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.GroupID}. {group.GroupName}");
            }

            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int groupId;
            do
            {
                Console.Write("Select a Group to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(groups.Select(p => p.GroupID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    groupId = int.Parse(input);
                    break;
                }
            } while (true);

            do
            {
                PostsMenu postsMenu = new PostsMenu(_userId);
                Console.WriteLine("============================");
                Console.WriteLine("1. View Posts");
                Console.WriteLine("2. Create a Post");
                Console.WriteLine("3. Delete Group (Must be the Admin)");
                Console.WriteLine("4. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            postsMenu.ViewPosts(postService.GetGroupPosts(groupId));
                            break;
                        }
                    case "2":
                        {
                            postsMenu.CreatePost(groupId);
                            break;
                        }
                    case "3":
                        {
                            if (DeleteGroup(groupId))
                                return;
                            else break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid Option!");
                            return;
                        }


                }


            } while (true);
        }
        public bool DeleteGroup(int groupId)
        {
            try
            {
                Console.WriteLine("Are you sure you want to delete this Group?");
                Console.WriteLine("1. Yes\nElse. No");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                if (input == "1")
                {
                    groupService.DeleteGroup(_userId, groupId);
                    Console.WriteLine("Group deleted Successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine("Group was not deleted");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
