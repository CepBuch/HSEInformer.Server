using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.DTO
{
    public class DTOPost
    {
        public int Id { get; set; }

        public string Theme { get; set; }

        public string Content { get; set; }

        public DTOGroup Group { get; set; }

        public DTOUser User { get; set; }

        public DateTime Time { get; set; }
    }
}
