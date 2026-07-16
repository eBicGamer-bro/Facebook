using Data;
using Data.DTOs;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public class CommentService : Service
    {
        private UserService _userService;
        private PostService _postService;
        private NotificationService _notificationService;

        public CommentService()
        {
            _userService = new UserService();
            _postService = new PostService();
            _notificationService = new NotificationService();
        }

        public void AddComment(int userId, int postId, string content)
        {
            using var _db = new FacebookDbContext();

            if(string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Comment content cannot be empty.");
            }
            if(_userService.CheckID(userId) == false)
            {
                throw new ArgumentException("User does not exist.");
            }
            if(_postService.CheckID(postId) == false)
            {
                throw new ArgumentException("Post does not exist.");
            }

            var comment = new Comment
            {
                UserID = userId,
                PostID = postId,
                Message = content,
            };
            _db.Comments.Add(comment);
            if(userId != _db.Posts.First(p => p.PostID == postId).UserID)
            {
                var notification = new Notification
                {
                    UserID = _db.Posts.First(p => p.PostID == postId).UserID,
                    Message = $"User {_userService.GetUsername(_db.Posts.First(p => p.PostID == postId).UserID)} commented on your post.",
                    CreatedAt = DateTime.Now
                };
                _db.Notifications.Add(notification);
            }
            
            _db.SaveChanges();
        }

        public void ReplyToComment(int userId,int postId, int commentId, string content)
        {
            using var _db = new FacebookDbContext();
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Reply content cannot be empty.");
            }
            if (_userService.CheckID(userId) == false)
            {
                throw new ArgumentException("User does not exist.");
            }
            if (_postService.CheckID(postId) == false)
            {
                throw new ArgumentException("Post does not exist.");
            }
            if (CheckID(commentId) == false)
            {
                throw new ArgumentException("Comment does not exist.");
            }
            var reply = new Comment
            {
                UserID = userId,
                PostID = postId,
                ParentCommentID = commentId,
                Message = content,
            };
            _db.Comments.Add(reply);
            _db.SaveChanges();
        }

        public List<CommentsDto> GetComments(int postID)
        {
            if (!_postService.CheckID(postID))
            {
                throw new ArgumentException("Invalid PostID");
            }
            using var _db = new FacebookDbContext();

            return _db.Comments
                .Where(c => c.PostID == postID && c.ParentCommentID == null)
                .Select(c => new CommentsDto
                {
                    CommentID = c.CommentID,
                    UserID = c.UserID,
                    PostID = c.PostID,
                    CommentText = c.Message,
                    CreatorName = c.User.Username,
                })
                .ToList();
        }
        public List<CommentsDto> GetReplies(int commentId)
        {
            if (CheckID(commentId) == false)
            {
                throw new ArgumentException("Comment does not exist.");
            }
            using var _db = new FacebookDbContext();
            
            return _db.Comments
                .Where(c => c.ParentCommentID == commentId)
                .Select(c => new CommentsDto
                {
                    CommentID = c.CommentID,
                    UserID = c.UserID,
                    PostID = c.PostID,
                    CommentText = c.Message,
                    CreatorName = c.User.Username,
                })
                .ToList();

        }
        public override bool CheckID(int id)
        {
            using var _db = new FacebookDbContext();
            if (_db.Comments.Any(c => c.CommentID == id))
            {
                return true;
            }
            return false;
        }
    }
}
