using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace HSEInformer.Server.Models
{
    public static class DBInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            var context = serviceProvider.GetService<HSEInformerServerContext>();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Login = "aaavakyan@edu.hse.ru",
                        Name = "Армен",
                        Surname = "Авакян",
                        Patronymic = "Амаякович",
                        Password = "202CB962AC59075B964B07152D234B70"
                    }
                );
                context.SaveChanges();
            }

            if (!context.HseMembers.Any())
            {
                context.HseMembers.AddRange(
                    new HSEMember
                    {
                        Email = "sabuchko@edu.hse.ru",
                        Name = "Сергей",
                        Surname = "Бучко",
                        Patronymic = "Александрович",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-151",
                        IsFacultyStarosta = false,
                        IsGroupStarosta = false,
                        IsYearStarosta = false
                    },
                    new HSEMember
                    {
                        Email = "emtyukhova@edu.hse.ru",
                        Name = "Елена",
                        Surname = "Тюхова",
                        Patronymic = "Михайловна",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-151",
                        IsFacultyStarosta = false,
                        IsGroupStarosta = true,
                        IsYearStarosta = false
                    },
                    new HSEMember
                    {
                        Email = "imsereda@edu.hse.ru",
                        Name = "Иван",
                        Surname = "Середа",
                        Patronymic = "Михайлович",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-151",
                        IsFacultyStarosta = false,
                        IsGroupStarosta = false,
                        IsYearStarosta = false
                    },
                    new HSEMember
                    {
                        Email = "memerkushina@edu.hse.ru",
                        Name = "Мария",
                        Surname = "Меркушина",
                        Patronymic = "Евгеньевна",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-151",
                        IsFacultyStarosta = false,
                        IsGroupStarosta = false,
                        IsYearStarosta = false
                    },
                    new HSEMember
                    {
                        Email = "aaalayeva@edu.hse.ru",
                        Name = "Антонина",
                        Surname = "Алаева",
                        Patronymic = "Александровна",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-152",
                        IsFacultyStarosta = false,
                        IsGroupStarosta = true,
                        IsYearStarosta = false
                    }
                );
                context.SaveChanges();
            }

        }
    }
}
