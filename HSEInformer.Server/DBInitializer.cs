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
                        Password = "123"
                    },
                      new User
                      {
                          Login = "sabuchko@edu.hse.ru",
                          Name = "Бучко",
                          Surname = "Сергей",
                          Patronymic = "Александрович",
                          Password = "qwe"
                      }
                );
                context.SaveChanges();
            }

        }
    }
}
