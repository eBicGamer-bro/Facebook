using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Post : BaseEntity
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public int? GroupID { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public Group? Group { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
