using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public string Token { get; set; }


        [Required]
        public string Name { get; set; }


        [Required]
        public string Surname { get; set; }


        public string Patronymic { get; set; }


        public List<UserGroup> UserGroups { get; set; }

        public User()
        {
            UserGroups = new List<UserGroup>();
        }

    }

}
