using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public GroupType GroupType { get; set; }

        public int AdministratorId { get; set; }

        public User Administrator { get; set; }


        public List<UserGroup> UserGroups { get; set; }

        public Group()
        {
            UserGroups = new List<UserGroup>();
        }
    }

    public enum GroupType
    {
        AutoCreated,
        Custom
    }
   
}
