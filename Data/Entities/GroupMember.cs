using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class GroupMember : BaseEntity
    {
        public int GroupMemberID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }

    }
}
