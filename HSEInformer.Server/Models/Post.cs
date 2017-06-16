using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Theme { get; set; }

        public string Content { get; set; }

        public Group Group { get; set; }

        public User User { get; set; }

        public DateTime Time { get; set; }
    }
}
