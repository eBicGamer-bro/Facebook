using Data.DTOs;
using Data.Entities;
using Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Menus
{
    public class CommentsMenu
    {
        private CommentService _commentService;
        public CommentsMenu()
        {
            _commentService = new CommentService();
        }

        public void ViewComments(List<CommentsDto> comments)
        {
            if (comments.Count == 0)
            {
                Console.WriteLine("No comments/replies");
                return;
            }
            Console.WriteLine("============================");
            Console.WriteLine("Comments: ");
            foreach (var comment in comments)
                Console.WriteLine($"{comment.CommentID}. {comment.CommentText}");
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");
            int commentID;
            do
            {
                Console.Write("Select a comment to interact with: ");
                string input = Console.ReadLine();
                if (Validation.Exit(input))
                {
                    return;
                }
                if (!Validation.CheckIDinList(comments.Select(c => c.CommentID).ToList(), input))
                {
                    Console.WriteLine("Invalid ID");
                }
                else
                {
                    commentID = int.Parse(input);
                    break;
                }

            } while (true);
            CommentsDto comment1 = comments.First(c => c.CommentID == commentID);
            do
            {
                Console.WriteLine("============================");
                Console.WriteLine("1. View Replies");
                Console.WriteLine("2. Reply");
                Console.WriteLine("3. Back");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        {
                            ViewComments(_commentService.GetReplies(commentID));
                            return;
                        }
                    case "2":
                        {
                            ReplytoComment(comment1.PostID, comment1.UserID, commentID);
                            return;
                        }
                    case "3":
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
        public void CommentonPost(int postId, int userID)
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");

            do
            {
                try
                {
                    Console.Write("Enter your comment: ");
                    string input = Console.ReadLine();
                    if (Validation.Exit(input))
                    {
                        return;
                    }
                    _commentService.AddComment(userID, postId, input);
                    Console.WriteLine("Comment posted successfully!");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }



            } while (true);
        }

        public void ReplytoComment(int postId, int userID, int commentID)
        {
            Console.WriteLine("============================");
            Console.WriteLine("Hint: Type 'exit' if you want to return!");

            do
            {
                try
                {
                    Console.Write("Enter your reply: ");
                    string input = Console.ReadLine();
                    if (Validation.Exit(input))
                    {
                        return;
                    }
                    _commentService.ReplyToComment(userID, postId, commentID, input);
                    Console.WriteLine("Reply posted successfully!");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (true);
        }
    }
}
