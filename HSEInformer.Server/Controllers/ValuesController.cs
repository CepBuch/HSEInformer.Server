using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HSEInformer.Server.Models;
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
        [Route("getGroups")]
        public IActionResult GetGroups()
        {
            //Получаем из токена username
            var username = User.Identity.Name;

            //ищем данного пользователя
            var user = _context.Users
                   .Include(g => g.UserGroups)
                   .ThenInclude(ug => ug.Group)
                   .FirstOrDefault(u => u.Username == username);


            if (user != null)
            {
                return Json(new { Ok = true, Result = user.UserGroups.Select(u => u.Group) });
            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
