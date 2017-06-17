using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    
    public class HSEMember
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string Patronymic { get; set; }

        [Required]
        public MemberType MemberType { get; set; }

        public StudyType StudyType { get; set; }

        public int StartDate { get; set; }

        public string Faculty { get; set; }

        public string Group { get; set; }

        public bool IsGroupStarosta { get; set; }

        public bool IsFacultyStarosta { get; set; }

        public bool IsYearStarosta { get; set; }



    }
    
    public enum MemberType
    {
        Student,
        Employee
    }

    public enum StudyType
    {
        Baccalaureate,
        Magistracy
    }
}
