using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class UserProfile : BaseEntity
    {
        [Key]
        public int UserID { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string? Bio { get; set; }
        public User User { get; set; }

    }
}
