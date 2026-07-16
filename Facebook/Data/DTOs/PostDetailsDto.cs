using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class PostDetailsDto
    {
        public int PostID { get; set; }
        public string PostText { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public List<string> Likes { get; set; }
        public List<CommentsDto> Comments { get; set; }
    }
}
