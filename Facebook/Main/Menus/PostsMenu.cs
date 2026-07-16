using Data.DTOs;
using Data.Entities;
using Main.Services;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menus
{
    public class PostsMenu : IMainMenu
    {
        private PostService postService;
        private CommentsMenu commentsMenu;
        private int _userId;
        public PostsMenu(int userId)
        {
            postService = new PostService();
            commentsMenu = new CommentsMenu();
            _userId = userId;
        }

        public void MainMenu()
        {
            do
            {


                Console.WriteLine("============================");
                Console.WriteLine("Posts Menu");
                Console.WriteLine("============================");
                Console.WriteLine("1. Create Post");
                Console.WriteLine("2. View All Posts");
                Console.WriteLine("3. View Your Posts");
                Console.WriteLine("4. Search For a Post By a Keyword");
                Console.WriteLine("5. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            CreatePost();
                            break;
                        }
                    case "2":
                        {
                            ViewPosts(postService.GetAllPosts()); 
                            break;
                        }
                    case "3":
                        {
                            ViewYourPosts();
                            break;
                        }
                    case "4":
                        {
                            SearchPostByKey();
                            break;
                        }
                    case "5":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid Choice!");
                            break;
                        }
                }

            } while (true);
        }

        public void SearchPostByKey()
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");

            
                Console.Write("Enter the keyword: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                ViewPosts(postService.GetPostsByKeyWord(input));
        }
        public void CreatePost(int? groupId = null)
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            do
            {
                try
                {
                    Console.Write("Enter your Post Content: ");
                    string content = Console.ReadLine();
                    if (Validation.Exit(content))
                    {
                        return;
                    }
                    postService.AddPost(_userId, content, groupId);
                    Console.WriteLine("Post Created Successfully!");
                    return;
                }catch(ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }

            }while (true);
            

        }

        public void UpdatePost(int postId)
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");

            do
            {
                try
                {
                    Console.Write("Enter your new Post Content: ");
                    string content = Console.ReadLine();
                    if (Validation.Exit(content))
                    {
                        return;
                    }
                    postService.UpdatePost(content, postId);
                    Console.WriteLine("Post Updated Successfully!");
                    return;
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (true);
        }

        public void DeletePost(int postId)
        {
            try
            {
                Console.WriteLine("Are you sure you want to delete this post?");
                Console.WriteLine("1. Yes\nElse. No");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                if (input == "1")
                {
                    postService.DeletePost(postId);
                    Console.WriteLine("Post deleted Successfully");
                }
                else Console.WriteLine("Post was not deleted");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ViewPosts(List<PostDetailsDto> posts)
        {
            if(posts.Count == 0)
            {
                Console.WriteLine("No posts exist");
                return;
            }
            Console.WriteLine("============================");
            foreach (var post in posts)
            {
                Console.WriteLine($"{post.PostID}. {post.PostText}.");
            }
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int postID;
            do
            {
                Console.Write("Select a Post to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(posts.Select(p => p.PostID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    postID = int.Parse(input);
                    break;
                }
            } while (true);
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. View Post Details");
                Console.WriteLine("2. Like");
                Console.WriteLine("3. Comment");
                Console.WriteLine("4. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            ViewPostDetails(postID);
                            break;
                        }
                    case "2":
                        {
                            try
                            {
                                postService.LikePost(_userId, postID);
                                Console.WriteLine("Post Liked!");
                                break;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                break;
                            }
                        }
                    case "3":
                        {
                            commentsMenu.CommentonPost(postID,_userId);
                            break;
                        }
                    case "4":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid Input");
                            break;
                        }

                }
             } while (true);
            
        }


        public void ViewYourPosts()
        {
            var posts = postService.GetUsersPosts(_userId);
            if(posts.Count == 0)
            {
                Console.WriteLine("There are no posts!");
                return;
            }
            foreach (var post in posts)
            {
                Console.WriteLine($"{post.PostID}. {post.PostText}");
            }
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int postID;
            do
            {
                Console.Write("Select a Post to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(posts.Select(p => p.PostID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    postID = int.Parse(input);
                    break;
                }

            } while (true);

            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. Update Post");
                Console.WriteLine("2. Delete Post");
                Console.WriteLine("3. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            UpdatePost(postID);
                            break;
                        }
                    case "2":
                        {
                            DeletePost(postID);
                            return;
                        }
                    case "3":
                        {
                            return;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid Input");
                            break;
                        }

                }
            } while (true);


        }

        public void ViewPostDetails(int postID)
        {
            var post = postService.GetPostDetails(postID);
            Console.WriteLine("============================");
            Console.WriteLine($"Creator Name: {post.CreatorName}");
            Console.WriteLine($"Created At: {post.CreatedAt}"); 
            Console.WriteLine($"{post.LikesCount} likes");
            Console.Write("Liked by: ");
            foreach (var like in post.Likes)
                Console.Write(like + ", ");
            Console.WriteLine("");
            commentsMenu.ViewComments(post.Comments);
        }
    }
}
