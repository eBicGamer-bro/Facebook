using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Notification : BaseEntity
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; }

    }
}
