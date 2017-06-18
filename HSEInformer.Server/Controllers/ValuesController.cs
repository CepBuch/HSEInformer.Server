using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HSEInformer.Server.Models;
using HSEInformer.Server.DTO;
using Microsoft.EntityFrameworkCore;

namespace HSEInformer.Server.Controllers
{
    public class ValuesController : Controller
    {
        private readonly HSEInformerServerContext _context;

        public ValuesController(HSEInformerServerContext context)
        {
            _context = context;
        }

        [Authorize]
        [Route("getProfile")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }


        [Authorize]
        [HttpGet]
        [Route("getGroups")]
        public IActionResult GetUserGroups()
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(g => g.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);


            if (user != null)
            {
                //Возвращаем его группы
                var groups = user.UserGroups.Select(ug => ug.Group).ToList();
                return Json(new
                {
                    Ok = true,
                    Result = groups
                    .Select(g => new DTOGroup { Id = g.Id, Name = g.Name, GroupType = (int)g.GroupType })
                });
            }
            else
            {
                return Unauthorized();
            }
        }




        [Authorize]
        [HttpGet]
        [Route("getGroupContent")]
        public IActionResult GetGroupContent([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная гуппа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;
            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {
                //Сообщения в группe
                var posts = _context.Posts.Where(p => p.Group.Id == id).ToArray();

                //Люди в группе
                var groupMembers = group.UserGroups.Select(ug => ug.User).ToArray();

                PostPermissionRequest[] requests = new PostPermissionRequest[0];
                PostPermission[] permissons = new PostPermission[0];
                bool isAdministrator = false;

                //Если пользователь - админ, то запросы на публикацию и люди, которые могут публиковать
                if (group.Administrator != null && group.Administrator.Id == user.Id)
                {
                    isAdministrator = true;
                    requests = _context.PostPermissionRequests
                        .Include(r => r.User)
                        .Where(r => r.Group.Id == id).ToArray();
                    permissons = _context.PostPermissions
                        .Include(r => r.User)
                        .Where(r => r.Group.Id == id).ToArray();
                }

                return Json(new
                {
                    Ok = true,
                    Result = new
                    {
                        IsAdministrator = isAdministrator,
                        Posts = posts.Select(p => new DTOPost
                        {
                            Id = p.Id,
                            Theme = p.Theme,
                            Content = p.Content,
                            Time = p.Time,
                            User = new DTOUser
                            {
                                Id = p.User.Id,
                                Username = p.User.Username,
                                Name = p.User.Name,
                                Surname = p.User.Surname,
                                Patronymic = p.User.Patronymic
                            }
                        }),
                        Members = groupMembers.Select(m => new DTOUser
                        {
                            Id = m.Id,
                            Username = m.Username,
                            Name = m.Name,
                            Surname = m.Surname,
                            Patronymic = m.Patronymic
                        }),
                        Requests = requests.Select(ppr => new DTOUser
                        {
                            Id = ppr.User.Id,
                            Username = ppr.User.Username,
                            Name = ppr.User.Name,
                            Surname = ppr.User.Surname,
                            Patronymic = ppr.User.Patronymic
                        }),
                        Permissons = permissons.Select(pp => new DTOUser
                        {
                            Id = pp.User.Id,
                            Username = pp.User.Username,
                            Name = pp.User.Name,
                            Surname = pp.User.Surname,
                            Patronymic = pp.User.Patronymic
                        })
                    }
                });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
