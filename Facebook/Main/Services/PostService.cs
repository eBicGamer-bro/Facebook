using Data;
using Data.DTOs;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public class PostService : Service
    {
        private UserService _userService;
        private FriendshipService _friendshipService;
        private NotificationService _notificationService;
        public PostService() 
        {
            _userService = new UserService();
            _friendshipService = new FriendshipService();
            _notificationService = new NotificationService();
        }

        public void AddPost(int userId, string content, int? groupId = null)
        {
            using var _db = new FacebookDbContext();
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Post content cannot be empty.");
            }
            if (!Validation.Post(content))
            {
                throw new ArgumentException("Post content is invalid.");
            }
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }
            var post = new Post
            {
                GroupID = groupId,
                UserID = userId,
                Message = content
            };
            _db.Posts.Add(post);
            _db.SaveChanges();
        }

        
        public void UpdatePost(string content, int postId)
        {
            if (!Validation.Post(content))
            {
                throw new ArgumentException("Post content is invalid.");
            }
            if (!CheckID(postId))
            {
                throw new ArgumentException("Post id does not exist");
            }
            using var _db = new FacebookDbContext();
            _db.Posts.Where(p => p.PostID == postId).ExecuteUpdate(p => p.SetProperty(post => post.Message, post => content));
        }

        public void DeletePost(int postId)
        {
            if (!CheckID(postId))
            {
                throw new ArgumentException("Post does not exist.");
            }

            using var _db = new FacebookDbContext();
            _db.Likes.Where(l => l.PostID == postId).ExecuteDelete();
            var commentsToDelete = _db.Comments.Where(c => c.PostID == postId).OrderByDescending(c => c.CommentID).ToList();
            for (int i = 0; i < commentsToDelete.Count; i++)//Not the best performance, but I can't use ExecuteDelete in one go as if there are replies the sql server could crash
            {
                _db.Comments.Where(c => c.CommentID == commentsToDelete[i].CommentID).ExecuteDelete();
            }
            _db.Posts.Where(p => p.PostID == postId).ExecuteDelete();
        }

        public void LikePost(int userId, int postId)
        {
            if (!CheckID(postId))
            {
                throw new ArgumentException("Post does not exist.");
            }

            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist");
            }

            using var _db = new FacebookDbContext();
            if (_db.Likes.Any(l => l.UserID == userId && l.PostID == postId))
            {
                throw new ArgumentException("User has already liked this post.");
            }
            var like = new Like
            {
                UserID = userId,
                PostID = postId
            };
            _db.Likes.Add(like);

            if(userId != _db.Posts.First(p => p.PostID == postId).UserID)
            {
                var notification = new Notification
                {
                    UserID = _db.Posts.First(p => p.PostID == postId).UserID,
                    Message = $"{_userService.GetUsername(_db.Posts.First(p => p.PostID == postId).UserID)} liked your post.",
                    IsRead = false
                };
                _db.Notifications.Add(notification);
            }
            _db.SaveChanges();
        }

        public PostDetailsDto GetPostDetails(int postId)
        {
            if (!CheckID(postId))
            {
                throw new ArgumentException("Post does not exist.");
            }

            using var _db = new FacebookDbContext();
            var post = _db.Posts.Include(p => p.User).Include(p => p.Likes).ThenInclude(l => l.User).Include(p => p.Comments).ThenInclude(c => c.User).First(p => p.PostID == postId);
            var postDetailsDto = new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            };

            return postDetailsDto;
        }

        public List<PostDetailsDto> GetFriendsPosts(int userId)
        {
            using var _db = new FacebookDbContext();
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist");
            }
            var friendsIds = _friendshipService.GetFriendsList(userId).Select(f => f.FriendID).ToList();
            var posts = _db.Posts.Include(p => p.User).Include(p => p.Likes).ThenInclude(l => l.User).Include(p => p.Comments).ThenInclude(c => c.User).Where(p => friendsIds.Contains(p.UserID)).ToList();
             var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public List<PostDetailsDto> GetGroupPosts(int groupId)
        {
            using var _db = new FacebookDbContext();
            if (!_db.Groups.Any(g => g.GroupID == groupId))
            {
                throw new ArgumentException("Group ID does not exist");
            }
            var posts = _db.Posts.Include(p => p.User).Include(p => p.Likes).ThenInclude(l => l.User).Include(p => p.Comments).ThenInclude(c => c.User).Where(p => p.GroupID == groupId).ToList();
            var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public List<PostDetailsDto> GetOrderedPosts()
        {
            using var _db = new FacebookDbContext();
            var posts = _db.Posts.Include(p => p.User).Include(p => p.Likes).ThenInclude(l => l.User).Include(p => p.Comments).ThenInclude(c => c.User).OrderByDescending(p => p.CreatedAt).ToList();
            var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public List<PostDetailsDto> GetPostsByKeyWord(string keyWord)
        {
            using var _db = new FacebookDbContext();
            var posts = _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Where(p => p.Message.Contains(keyWord))
                .ToList();
            var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public List<PostDetailsDto> GetAllPosts()
        {
            using var _db = new FacebookDbContext();
            var posts = _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .ToList();
            var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public List<PostDetailsDto> GetUsersPosts(int userID)
        {
            if (!_userService.CheckID(userID))
            {
                throw new ArgumentException("User Id does not exist");
            }
            using var _db = new FacebookDbContext();
            var posts = _db.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .ThenInclude(l => l.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Where(p => p.UserID == userID)
                .ToList();
              
            var postsDetailsDto = posts.Select(post => new PostDetailsDto
            {
                PostID = post.PostID,
                PostText = post.Message,
                CreatorName = post.User.Username,
                LikesCount = post.Likes.Count,
                Likes = post.Likes.Select(l => l.User.Username).ToList(),
                Comments = post.Comments.Where(c => c.ParentCommentID == null).Select(c => new CommentsDto
                {
                    PostID = c.PostID,
                    UserID = c.UserID,
                    CommentID = c.CommentID,
                    CreatorName = c.User.Username,
                    CommentText = c.Message,
                }).ToList()
            }).ToList();
            return postsDetailsDto;
        }

        public override bool CheckID(int id)
        {
            using var _db = new FacebookDbContext();
            if (_db.Posts.Any(p => p.PostID == id))
            {
                return true;
            }
            return false;
        }
    }
}
