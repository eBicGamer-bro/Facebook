using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class FriendProfileDto
    {
        public int FriendId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Posts { get; set; }

    }
}
