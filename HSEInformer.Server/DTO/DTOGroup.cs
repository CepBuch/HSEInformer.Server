using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.DTO
{
    public class DTOGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int GroupType { get; set; }

        public DTOUser Administrator { get; set; }
    }
}
