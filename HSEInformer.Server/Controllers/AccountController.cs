using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSEInformer.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using HSEInformer.Server.Interfaces;

namespace HSEInformer.Server.Controllers
{
    //[Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly HSEInformerServerContext _context;

        public AccountController(HSEInformerServerContext context)
        {
            _context = context;
        }

        // GET: сheckUser?email=sabuchko@edu.hse.ru
        [AllowAnonymous]
        [HttpGet("/checkUser")]
        public async Task<IActionResult> CheckUser([FromQuery]string email)
        {
            if (!(email.EndsWith("edu.hse.ru") || email.EndsWith("hse.ru")))
            {
                return BadRequest("Email should be ended with edu.hse.ru or hse.ru");
            }

            var member = FindHseMember(email);
            var memberExists = member != null;
            if (memberExists)
            {
                IConfirmationCodeGenerator generator = new CodeGenerator();
                var code = generator.Generate();

                if (!string.IsNullOrWhiteSpace(code))
                {
                    IEmailManager manager = new EmailManager();
                    var sendMailStatus = await manager.SendConfirmationEmailAsync(email, code);

                    if (sendMailStatus)
                    {
                        AddConfirmation(member, code);
                    }
                    else
                    {
                        return BadRequest("Email cannoot be sent to a server");
                    }

                }
            }
            return Json(new { exists = memberExists });
        }

        private User FindHseMember(string email)
        {
            return _context.Users.FirstOrDefault(m => m.Login.ToLower() == email.ToLower().Trim());
        }

        private void AddConfirmation(User user, string code)
        {
            //Получал ли данный пользователь уже код подтверждения
            var foundUser = _context.Confirmations
                .FirstOrDefault(m => m.User != null && m.User.Login.ToLower() == user.Login.Trim().ToLower());

            //Если получал, то обновляем его код новым. Если нет, то записываем в бд ждущих код
            if (foundUser != null)
            {
                foundUser.Code = code;
            }
            else
            {
                _context.Confirmations.Add(new Confirmation { User = user, Code = code });

            }
            _context.SaveChanges();
        }


        public class ConfirmationInfo
        {
            public string Email { get; set; }

            public string Code { get; set; }
        }


        [AllowAnonymous]
        [HttpPost("/ConfirmEmail")]
        public IActionResult ConfirmEmail([FromBody]ConfirmationInfo confirmation)
        {
            var user = GetUserByCode(confirmation.Email, confirmation.Code);

            if (user != null)
            {
                return Json(new { Ok = true, User = user });
            }
            else
            {
                return Json(new { Ok = false, Message = "The confirmation code is not correct" });
            }
        }

        private User GetUserByCode(string email, string code)
        {
            var confirmation = _context.Confirmations.Include(c => c.User)
                .FirstOrDefault(e => e.User.Login.ToLower() == email.ToLower().Trim());

            if (confirmation != null && confirmation.Code.ToLower() == code.Trim().ToLower())
            {
                return confirmation.User;
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost("/token")]
        public async Task Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            // сериализация ответа
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }


        

    }
}