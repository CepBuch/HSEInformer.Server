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

            var lena = new User
            {
                Username = "emtyukhova@edu.hse.ru",
                Name = "Елена",
                Surname = "Тюхова",
                Patronymic = "Михайловна",
                Password = "202CB962AC59075B964B07152D234B70"
            };

            var tonya = new User
            {
                Username = "aaalayeva@edu.hse.ru",
                Name = "Антонина",
                Surname = "Алаева",
                Patronymic = "Александровна",
                Password = "202CB962AC59075B964B07152D234B70"
            };

            var vika = new User
            {
                Username = "vdsemakova@edu.hse.ru",
                Name = "Виктория",
                Surname = "Семакова",
                Patronymic = "Дмитриевна",
                Password = "202CB962AC59075B964B07152D234B70"
            };

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "aaavakyan@edu.hse.ru",
                        Name = "Армен",
                        Surname = "Авакян",
                        Patronymic = "Амаякович",
                        Password = "202CB962AC59075B964B07152D234B70"
                    },
                    lena,
                    new User
                    {
                        Username = "sefremov@hse.ru",
                        Name = "Сергей",
                        Surname = "Ефремов",
                        Patronymic = "Геннадьевич",
                        Password = "202CB962AC59075B964B07152D234B70"
                    },
                    tonya,
                    vika
                );
                context.SaveChanges();
            }

            if (!context.Groups.Any())
            {
                var group1 = new Group
                {
                    Name = "ББИ-151",
                    Administrator = lena,
                    GroupType = GroupType.AutoCreated,
                };
                context.Groups.Add(group1);
                context.SaveChanges();

                group1.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group1.Id,
                        UserId = 1
                    },
                       new UserGroup
                    {
                        GroupId = group1.Id,
                        UserId = 2
                    }
                };

                var group2 = new Group
                {
                    Name = "Бизнес-Информатика",
                    Administrator = vika,
                    GroupType = GroupType.AutoCreated,
                };
                context.Groups.Add(group2);
                context.SaveChanges();

                group2.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group2.Id,
                        UserId = 1,

                    },
                       new UserGroup
                    {
                        GroupId = group2.Id,
                        UserId = 2
                    },
                          new UserGroup
                    {
                        GroupId = group2.Id,
                        UserId = tonya.Id
                    },
                                new UserGroup
                    {
                        GroupId = group2.Id,
                        UserId = vika.Id
                    }
                };


                var group3 = new Group
                {
                    Name = "Бизнес-Информатика 2015",
                    Administrator = null,
                    GroupType = GroupType.AutoCreated,
                };
                context.Groups.Add(group3);
                context.SaveChanges();

                group3.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group3.Id,
                        UserId = 1
                    },
                       new UserGroup
                    {
                        GroupId = group3.Id,
                        UserId = 2
                    },
                          new UserGroup
                    {
                        GroupId = group3.Id,
                        UserId = tonya.Id
                    }
                };
                context.SaveChanges();


                var group4 = new Group
                {
                    Name = "Бизнес-Информатика 2014",
                    Administrator = null,
                    GroupType = GroupType.AutoCreated,
                };
                group4.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group4.Id,
                        UserId = vika.Id
                    }
                };
                context.Groups.Add(group4);
                context.SaveChanges();


                var group5 = new Group
                {
                    Name = "ББИ-141",
                    Administrator = null,
                    GroupType = GroupType.AutoCreated,
                };
                group5.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group5.Id,
                        UserId = vika.Id
                    },
                };
                context.Groups.Add(group5);
                context.SaveChanges();

                var group6 = new Group
                {
                    Name = "ББИ-152",
                    Administrator = tonya,
                    GroupType = GroupType.AutoCreated,
                };
                group6.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group6.Id,
                        UserId = tonya.Id
                    }
                };
                context.Groups.Add(group6);
                context.SaveChanges();


                context.Posts.AddRange(
                   new Post
                   {
                       Group = group1,
                       Theme = "Знакомимся",
                       Time = DateTime.Now - TimeSpan.FromDays(2),
                       Content = "Добрый день, мои новые одногруппники! Я ваша староста. Если я недоступна здесь, то звоните мне по номеру +79256410156",
                       User = lena
                   },
                    new Post
                    {
                        Group = group1,
                        Theme = "Открытие регистрации",
                        Time = DateTime.Now,
                        Content = "Всем привет! Администрация просила передать, что сегодня в 20:00 откроется регистрация на концерт группы СПЛИН.",
                        User = lena
                    });
                context.SaveChanges();

                if (!context.PostPermissions.Any())
                {
                    context.PostPermissions.AddRange(
                        new PostPermission
                        {
                            Group = group1,
                            User = lena
                        },
                        new PostPermission
                        {
                            Group = group6,
                            User = tonya
                        },
                        new PostPermission
                        {
                            Group = group2,
                            User = vika
                        });
                }
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
                        StartDate = 2015,
                        IsFacultyStarosta = false,
                        IsGroupStarosta = false,
                        IsYearStarosta = false
                    },
                    new HSEMember
                    {
                        Email = "aaavakyan@edu.hse.ru",
                        Name = "Армен",
                        Surname = "Авакян",
                        Patronymic = "Амаякович",
                        MemberType = MemberType.Student,
                        StudyType = StudyType.Baccalaureate,
                        Faculty = "Бизнес-Информатика",
                        Group = "ББИ-151",
                        StartDate = 2015,
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
                        StartDate = 2015,
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
                        StartDate = 2015,
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
                        StartDate = 2015,
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
                        StartDate = 2015,
                        IsFacultyStarosta = false,
                        IsGroupStarosta = true,
                        IsYearStarosta = false
                    },
                      new HSEMember
                      {
                          Email = "vasemakova@edu.hse.ru",
                          Name = "Виктория",
                          Surname = "Семакова",
                          Patronymic = "Александровна",
                          MemberType = MemberType.Student,
                          StudyType = StudyType.Baccalaureate,
                          Faculty = "Бизнес-Информатика",
                          Group = "ББИ-141",
                          StartDate = 2014,
                          IsFacultyStarosta = false,
                          IsGroupStarosta = false,
                          IsYearStarosta = false
                      },
                    new HSEMember
                    {
                        Email = "sefremov@hse.ru",
                        Name = "Сергей",
                        Surname = "Ефремов",
                        Patronymic = "Геннадьевич",
                        MemberType = MemberType.Employee,
                    }
                );
                context.SaveChanges();


            }

        }
    }
}
