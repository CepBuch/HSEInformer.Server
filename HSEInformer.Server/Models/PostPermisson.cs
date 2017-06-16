using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public class PostPermission
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Group Group { get; set; }
    }
}
