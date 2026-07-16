using Data;
using Data.DTOs;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Services
{
    public class NotificationService
    {
        private UserService _userService;
        public NotificationService()
        {
            _userService = new UserService();
        }
        public void AddNotification(string message, int userId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }
            if(string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Notification message cannot be empty.");
            }
            using var _db = new FacebookDbContext();
            var notification = new Notification
            {
                UserID = userId,
                Message = message,
                IsRead = false
            };
            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }

        public void MarkAllAsRead(int userId)
        {
            if(!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }
            using var _db = new FacebookDbContext();
            _db.Notifications.Where(n => n.UserID == userId && !n.IsRead).ExecuteUpdate(n => n.SetProperty(n => n.IsRead, true));
        }

        public List<NotificationDto> GetNotifications(int userId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User does not exist.");
            }
            using var _db = new FacebookDbContext();
            return _db.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .Select(n => new NotificationDto
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }
    }
}
