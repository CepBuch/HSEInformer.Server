using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HSEInformer.Server.Models;
using HSEInformer.Server.DTO;
using Microsoft.EntityFrameworkCore;
using HSEInformer.Server.ViewModels;

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
        [Route("checkIfAdmin")]
        public IActionResult CheckIfAdmin([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {

                return Json(new
                {
                    Ok = true,
                    Result = (group.Administrator != null && group.Administrator.Id == user.Id),
                });
            }
            else
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpGet]
        [Route("getAdministrator")]
        public IActionResult GetAdministrator([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {

                return Json(new
                {
                    Ok = true,
                    Result = group.Administrator != null ? new DTOUser
                    {
                        Name = group.Administrator.Name,
                        Surname = group.Administrator.Surname,
                        Patronymic = group.Administrator.Patronymic,
                        Username = group.Administrator.Username
                    }
                    :
                    null
                });
            }
            else
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpGet]
        [Route("getPosts")]
        public IActionResult GetPosts([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;


            if (user != null && userIsInGroup)
            {
                //Сообщения в группe
                var posts = _context.Posts
                    .Include(p => p.User)
                    .Where(p => p.Group.Id == id).ToArray();

                return Json(new
                {
                    Ok = true,
                    Result = posts.Select(p => new DTOPost
                    {
                        Id = p.Id,
                        Theme = p.Theme,
                        Content = p.Content,
                        Time = p.Time,
                        User = new DTOUser
                        {
                            Username = p.User.Username,
                            Name = p.User.Name,
                            Surname = p.User.Surname,
                            Patronymic = p.User.Patronymic
                        }
                    })
                });
            }
            else
            {
                return Unauthorized();
            }
        }



        [Authorize]
        [HttpGet]
        [Route("getFeed")]
        public IActionResult GetFeed()
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                //Индексы групп, в которых есть данный пользователь
                var groups = user.UserGroups.Select(ug => ug.Group.Id).ToList();

                //Посты из сообществ, в котором есть юзер
                var posts = _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Group)
                    .Where(p => groups.Contains(p.Group.Id))
                    .OrderByDescending(p => p.Time)
                    .Take(50)
                    .ToArray();
 
                return Json(new
                {
                    Ok = true,
                    Result = posts.Select(p => new DTOPost
                    {
                        Id = p.Id,
                        Theme = p.Theme,
                        Content = p.Content,
                        Time = p.Time,
                        User = new DTOUser
                        {
                            Username = p.User.Username,
                            Name = p.User.Name,
                            Surname = p.User.Surname,
                            Patronymic = p.User.Patronymic
                        }
                    })
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getGroupMembers")]
        public IActionResult GetGroupMembers([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {
                //Люди в группе
                var groupMembers = group.UserGroups.Select(ug => ug.User).ToArray();

                return Json(new
                {
                    Ok = true,
                    Result = groupMembers.Select(m => new DTOUser
                    {
                        Username = m.Username,
                        Name = m.Name,
                        Surname = m.Surname,
                        Patronymic = m.Patronymic
                    })
                });
            }
            else
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpGet]
        [Route("getGroupPostPermissions")]
        public IActionResult GetGroupPostPermissions([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {
                var postPermissions = _context.PostPermissions
                     .Include(r => r.User)
                     .Where(r => r.Group.Id == id);
                var usersWithPermissions = postPermissions.Select(m => new DTOUser
                {
                    Username = m.User.Username,
                    Name = m.User.Name,
                    Surname = m.User.Surname,
                    Patronymic = m.User.Patronymic
                }).ToList();

                return Json(new
                {
                    Ok = true,
                    Result = usersWithPermissions
                });
            }
            else
            {
                return Unauthorized();
            }
        }



        [Authorize]
        [HttpGet]
        [Route("getGroupPostPermissionRequests")]
        public IActionResult GetGroupPostPermissionRequests([FromQuery]int id)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //Ищем, есть ли у пользователя данная группа
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);

            //Если пользователь -админ, то запросы на публикацию и люди, которые могут публиковать
            if (user != null && group != null && group.Administrator != null && group.Administrator.Id == user.Id)
            {
                var postPermissionRequests = _context.PostPermissionRequests
                    .Include(r => r.User)
                    .Where(r => r.Group.Id == id).ToArray();

                return Json(new
                {
                    Ok = true,
                    Result =
                         postPermissionRequests.Select(m => new DTOUser
                         {
                             Username = m.User.Username,
                             Name = m.User.Name,
                             Surname = m.User.Surname,
                             Patronymic = m.User.Patronymic
                         })
                });
            }
            else
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpGet]
        [Route("getUserPostPermissions")]
        public IActionResult GetUserPostPermissions()
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                //Группы, в которые пользователь может писать
                var groups = _context.PostPermissions
                    .Include(p => p.User)
                    .Include(p => p.Group)
                    .Where(p => p.User.Id == user.Id)
                    .Select(p => p.Group)
                    .ToList();

                
                var groupsUserCanPost = groups.Select(g => new DTOGroup
                {
                    Id = g.Id,
                    Name = g.Name,
                    GroupType = (int)g.GroupType
                }).ToList();

                return Json(new
                {
                    Ok = true,
                    Result = groupsUserCanPost
                });


            }
            else
            {
                return Unauthorized();
            }
        }



        [Authorize]
        [HttpPost]
        [Route("sendMessage")]
        public IActionResult ConfirmEmail([FromBody]PostViewModel model)
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //Ищем данного пользователя
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

           

            if (user != null)
            {

                int group_id = model.Group_id;
                string theme = model.Theme;
                string content = model.Content;


                //Существует ли такая группа
                var group = _context.Groups
                   .Include(g => g.UserGroups)
                   .ThenInclude(ug => ug.User)
                   .FirstOrDefault(g => g.Id == group_id);

                //Имеет ли данный пользователь право публиковать запись
                var postPermission = _context.PostPermissions
                    .Include(p => p.User)
                    .FirstOrDefault(p => p.Group.Id == group_id && p.User.Id == user.Id);

                if (group!=null && postPermission != null)
                {
                    var post = new Post
                    {
                        Theme = theme,
                        Content = content,
                        User = user,
                        Group = group,
                        Time = DateTime.UtcNow
                    };
                    _context.Posts.Add(post);
                    _context.SaveChanges();

                    return Ok();
                }
                return Unauthorized();
            }
            else
            {
                return Unauthorized();
            }
        }
    }


}
