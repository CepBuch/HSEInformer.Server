using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public class UserGroup
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
