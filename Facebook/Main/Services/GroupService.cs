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
    public class GroupService : Service
    {
        private UserService _userService;
        private PostService _postService;
        public GroupService()
        {
            _userService = new UserService();
            _postService = new PostService();
        }
        public List<GroupDetailsDto> GetUsersGroups(int userId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist.");
            }
            using var _db = new FacebookDbContext();
            return _db.Groups
                .Include(g=>g.GroupMembers)
                .Where(g => g.GroupMembers.Any(m => m.UserID == userId))
                .Select(g => new GroupDetailsDto
                {
                    GroupID = g.GroupID,
                    GroupName = g.GroupName,
                    AdminID = g.AdminID
                    
                }).ToList();
        }

        public void JoinGroup(int userId, int groupId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist.");
            }
            if (!CheckID(groupId))
            {
                throw new ArgumentException("Group ID does not exist.");
            }
            using var _db = new FacebookDbContext();
            if (_db.GroupMembers.Any(m => m.UserID == userId && m.GroupID == groupId))
            {
                throw new InvalidOperationException("User is already a member of this group.");
            }
            var groupMember = new GroupMember
            {
                UserID = userId,
                GroupID = groupId
            };
            _db.GroupMembers.Add(groupMember);
            _db.SaveChanges();
        }

        public void LeaveGroup(int userId, int groupId)
        {
            if (!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist.");
            }
            if (!CheckID(groupId))
            {
                throw new ArgumentException("Group ID does not exist.");
            }
            using var _db = new FacebookDbContext();
            var groupMember = _db.GroupMembers.FirstOrDefault(m => m.UserID == userId && m.GroupID == groupId);
            if (groupMember == null)
            {
                throw new InvalidOperationException("User is not a member of this group.");
            }
            _db.GroupMembers.Remove(groupMember);
            _db.SaveChanges();
        }

        public void CreateGroup(int adminId, string groupName)
        {
            if (!_userService.CheckID(adminId))
            {
                throw new ArgumentException("Admin ID does not exist.");
            }
            using var _db = new FacebookDbContext();
            var group = new Group
            {
                GroupName = groupName,
                AdminID = adminId
            };
            _db.Groups.Add(group);
            _db.SaveChanges();
            var groupMember = new GroupMember
            {
                UserID = adminId,
                GroupID = group.GroupID
            };
            _db.GroupMembers.Add(groupMember);
            _db.SaveChanges();
        }

        public void DeleteGroup(int userId, int groupId)
        {
            if (!CheckID(groupId))
            {
                throw new ArgumentException("Group ID does not exist.");
            }
            if(!_userService.CheckID(userId))
            {
                throw new ArgumentException("User ID does not exist.");
            }
            using var _db = new FacebookDbContext();
            var group = _db.Groups.Include(g => g.Posts).First(g => g.GroupID == groupId);
            if (group.AdminID != userId)
            {
                throw new InvalidOperationException("Only the admin can delete the group.");
            }
            _db.GroupMembers.Where(m => m.GroupID == groupId).ExecuteDelete();
            foreach (var post in group.Posts.ToList())
            {
                _postService.DeletePost(post.PostID);
            }
            _db.Groups.Where(g=>g.GroupID == groupId).ExecuteDelete();
        }

        public List<GroupDetailsDto> GetAllGroups()
        {
            using var _db = new FacebookDbContext();
            return _db.Groups
                .Include(g => g.GroupMembers)
                .Select(g => new GroupDetailsDto
                {
                    GroupID = g.GroupID,
                    GroupName = g.GroupName,
                    AdminID = g.AdminID

                }).ToList();
        }
        public override bool CheckID(int id)
        {
            using var _db = new FacebookDbContext();

            if(_db.Groups.Any(g => g.GroupID == id))
            {
                return true;
            }
            return false;
        }

    }
}
