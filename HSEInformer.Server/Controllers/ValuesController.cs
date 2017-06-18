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
            return Ok($"��� �����: {User.Identity.Name}");
        }


        [Authorize]
        [HttpGet]
        [Route("getGroups")]
        public IActionResult GetUserGroups()
        {
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(g => g.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);


            if (user != null)
            {
                //���������� ��� ������
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
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //����, ���� �� � ������������ ������ ������
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
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //����, ���� �� � ������������ ������ ������
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
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

            //����, ���� �� � ������������ ������ ������
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;


            if (user != null && userIsInGroup)
            {
                //��������� � �����e
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
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                //������� �����, � ������� ���� ������ ������������
                var groups = user.UserGroups.Select(ug => ug.Group.Id).ToList();

                //����� �� ���������, � ������� ���� ����
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
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //����, ���� �� � ������������ ������ ������
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);


            if (user != null && userIsInGroup && group != null)
            {
                //���� � ������
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
        [Route("getPostPermissions")]
        public IActionResult GetPostPermissions([FromQuery]int id)
        {
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //����, ���� �� � ������������ ������ ������
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
        [Route("getPostPermissionRequests")]
        public IActionResult GetPostPermissionRequests([FromQuery]int id)
        {
            //�������� �� ������ username
            var username = User.Identity.Name;

            //���� ������� ������������
            var user = _context.Users
                   .Include(u => u.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);
            //����, ���� �� � ������������ ������ ������
            var usergroup = user.UserGroups
                .FirstOrDefault(ug => ug.GroupId == id);
            var userIsInGroup = usergroup != null;

            var group = _context.Groups
               .Include(g => g.UserGroups)
               .ThenInclude(ug => ug.User)
               .FirstOrDefault(g => g.Id == id);

            //���� ������������ -�����, �� ������� �� ���������� � ����, ������� ����� �����������
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
    }


}
