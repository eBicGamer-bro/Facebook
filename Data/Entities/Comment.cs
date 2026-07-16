using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Comment : BaseEntity
    {
        public int CommentID { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
        public int? ParentCommentID { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
