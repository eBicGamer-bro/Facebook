using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User : BaseEntity
    {
        public int UserID { get; set; }
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }
        public string Email { get; set; }
        [MinLength(8)]
        public string Password { get; set; }
        public UserProfile UserProfile { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();       
        public ICollection<Group> GroupsOwned { get; set; } = new List<Group>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
