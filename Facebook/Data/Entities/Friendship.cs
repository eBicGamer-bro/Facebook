using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Friendship : BaseEntity
    {
        public int FriendshipID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public Status Status { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
