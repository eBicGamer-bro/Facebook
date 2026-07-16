using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Group : BaseEntity
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int AdminID { get; set; }
        public User Admin { get; set; }
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
