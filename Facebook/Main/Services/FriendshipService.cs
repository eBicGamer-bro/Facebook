using Data;
using Data.DTOs;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public class FriendshipService : Service
    {
        private UserService _userService;
        public FriendshipService()
        {
            _userService = new UserService();
        }

        public void SendFriendRequest(int senderId, int receiverId)
        {
            using var _db = new FacebookDbContext();
            if (!_userService.CheckID(senderId))
            {
                throw new ArgumentException($"Sender with ID {senderId} does not exist.");
            }
            if (!_userService.CheckID(receiverId))
            {
                throw new ArgumentException($"Receiver with ID {receiverId} does not exist.");
            }
            if(_db.Friendships.Any(f => (f.SenderID == senderId && f.ReceiverID == receiverId) || (f.SenderID == receiverId && f.ReceiverID == senderId)))
            {
                throw new InvalidOperationException("A friendship already exists between these users.");
            }
            if(senderId == receiverId)
            {
                throw new InvalidOperationException("A user can't add itself");
            }
            var friendship = new Friendship
            {
                SenderID = senderId,
                ReceiverID = receiverId,
                Status = Status.Pending
            };
            _db.Friendships.Add(friendship);
            _db.SaveChanges();
        }

        public void AcceptFriendRequest(int friendshipId)
        {
            using var _db = new FacebookDbContext();
            if (!CheckID(friendshipId))
            {
                throw new ArgumentException($"Friendship with ID {friendshipId} does not exist.");
            }
            var friendship = _db.Friendships.First(f => f.FriendshipID == friendshipId);
            if (friendship.Status != Status.Pending)
            {
                throw new InvalidOperationException("Only pending friend requests can be accepted.");
            }
            friendship.Status = Status.Approved;
            var notification = new Notification
            {
                UserID = friendship.SenderID,
                Message = $"User {_userService.GetUsername(friendship.ReceiverID)} accepted your friend request.",
                IsRead = false
            };
            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }
        public void RejectFriendRequest(int friendshipId)
        {
            using var _db = new FacebookDbContext();
            if (!CheckID(friendshipId))
            {
                throw new ArgumentException($"Friendship with ID {friendshipId} does not exist.");
            }
            var friendship = _db.Friendships.First(f => f.FriendshipID == friendshipId);
            if (friendship.Status != Status.Pending)
            {
                throw new InvalidOperationException("Only pending friend requests can be rejected.");
            }
            friendship.Status = Status.Rejected;
            _db.SaveChanges();
        }

        public void RemoveFriendship(int friendshipId)
        {
            using var _db = new FacebookDbContext();
            if (!CheckID(friendshipId))
            {
                throw new ArgumentException($"Friendship with ID {friendshipId} does not exist.");
            }
            _db.Friendships.Where(f => f.FriendshipID == friendshipId).ExecuteDelete();
        }
        public List<FriendshipDto> GetFriendsList(int userId)
        {
            using var _db = new FacebookDbContext();
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist!");
            }
            var friendsList = _db.Friendships.Include(f => f.Receiver).Where(f => (f.SenderID == userId || f.ReceiverID == userId) && f.Status == Status.Approved).Select(f => new FriendshipDto
            {
                FriendshipID = f.FriendshipID,
                FriendID = userId == f.SenderID ? f.ReceiverID : f.SenderID,
                FriendName = userId == f.SenderID ? f.Receiver.Username : f.Sender.Username
            }).ToList();

            return friendsList;
        }
        public List<FriendshipDto> GetPendingFriendRequests(int userId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist!");
            }

            using var _db = new FacebookDbContext();
            var pendingRequests = _db.Friendships.Include(f => f.Sender).Where(f => f.ReceiverID == userId && f.Status == Status.Pending).Select(f => new FriendshipDto
            {
                FriendshipID = f.FriendshipID,
                FriendID = f.SenderID,
                FriendName = f.Sender.Username
            }).ToList();
            return pendingRequests;
        }
        
        public List<UserDto> GetCommonFriends(int userId, int friendId)
        {
            if (!_userService.CheckID(userId) || !_userService.CheckID(friendId))
            {
                throw new ArgumentException("One or both users do not exist!");
            }
            using var _db = new FacebookDbContext();
            var userFriends = _db.Friendships.Where(f => (f.SenderID == userId || f.ReceiverID == userId) && f.Status == Status.Approved).Select(f => f.SenderID == userId ? f.ReceiverID : f.SenderID).ToList();
            var friendFriends = _db.Friendships.Where(f => (f.SenderID == friendId || f.ReceiverID == friendId) && f.Status == Status.Approved).Select(f => f.SenderID == friendId ? f.ReceiverID : f.SenderID).ToList();
            var commonFriendIds = userFriends.Intersect(friendFriends).ToList();
            var commonFriends = _db.Users.Where(u => commonFriendIds.Contains(u.UserID) && u.UserID != userId && u.UserID != friendId).Select(u => new UserDto
            {
                UserID = u.UserID,
                UserName = u.Username
            }).ToList();
            return commonFriends;
        } 
        public override bool CheckID(int id)
        {
            using var _db = new FacebookDbContext();
            if(_db.Friendships.Any(f => f.FriendshipID == id) ) 
            {
                return true;
            }
            return false;

        }
    }
}
