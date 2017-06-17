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
using HSEInformer.Server.ViewModels;

namespace HSEInformer.Server.Controllers
{
    [Produces("application/json")]
    public class AccountController : Controller
    {
        private readonly HSEInformerServerContext _context;

        public AccountController(HSEInformerServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// �����, �����������, ��������������� �� ��� ������������ � ������� HSEInformer.
        /// /checkAccountIsRegistred?email=sabuchko@edu.hse.ru
        /// </summary>
        /// <param name="email">Email �����������</param>
        /// <returns>true - ���������������, false - �� ���������������</returns>
        [AllowAnonymous]
        [HttpGet("/checkAccountIsRegistred")]
        public IActionResult CheckAccountIsRegistred(string email)
        {
            if (!(email.EndsWith("edu.hse.ru") || email.EndsWith("hse.ru")))
            {
                return Json(new { Ok = false, Message = "Email should be ended with edu.hse.ru or hse.ru" });
            }

            var user = _context.Users.FirstOrDefault(m => m.Username.ToLower() == email.ToLower().Trim());

            var memberExists = user != null;
            return Json(new { Ok = true, Result = memberExists });
        }


        // GET: /sendConfirmationCode?email=sabuchko@edu.hse.ru
        /// <summary>
        /// �����, �����������, ���� �� � ���� ������ ��� �������/��������� � ������ ������� ����������� �����.
        /// ���� ����, �� ������� ��������/���������� ������������ ��������� � 6-������� ����� ������������� �����������
        /// </summary>
        /// <param name="email">Email �����������</param>
        /// <returns>true - ����, false - ���</returns>
        [AllowAnonymous]
        [HttpGet("/sendConfirmationCode")]
        public async Task<IActionResult> CheckUserIsHseMember([FromQuery]string email)
        {
            if (!(email.EndsWith("@edu.hse.ru") || email.EndsWith("hse.ru")))
            {
                return Json(new { Ok = false, Message = "Email should be ended with @edu.hse.ru or @hse.ru" });
            }

            var user = _context.Users.Any(m => m.Username.ToLower() == email.ToLower().Trim());

            bool memberExists = false;
            if (!user)
            {
                var member = _context.HseMembers.FirstOrDefault(m => m.Email.ToLower() == email.ToLower().Trim());
                memberExists = member != null;
                if (memberExists)
                {
                    IConfirmationCodeGenerator generator = new CodeGenerator();
                    var code = generator.Generate();

                    IEmailManager manager = new EmailManager();
                    var sendMailStatus = await manager.SendConfirmationEmailAsync(email, code);

                    if (sendMailStatus)
                    {
                        AddConfirmation(member, code);
                    }
                    else
                    {
                        return Json(new { Ok = false, Message = "Confirmation email cannoot be sent from a server" });
                    }
                }
                return Json(new { Ok = true, Result = memberExists });
            }
            return Json(new { Ok = false, Message = "The user already eists" });

        }



        private void AddConfirmation(HSEMember member, string code)
        {
            //������� �� ������ ������������ ��� ��� �������������
            var foundUser = _context.Confirmations
                .FirstOrDefault(m => m.Member != null && m.Member.Email.ToLower() == member.Email.Trim().ToLower());

            //���� �������, �� ��������� ��� ��� �����. ���� ���, �� ���������� � �� ������ ���
            if (foundUser != null)
            {
                foundUser.Code = code;
            }
            else
            {
                _context.Confirmations.Add(new Confirmation { Member = member, Code = code });

            }
            _context.SaveChanges();
        }

        /// <summary>
        /// ����� ������������� ����, ������������� �� ����� ����������/�������� ���
        /// </summary>
        /// <param name="confirmation">������ confirmation: string Email, string Code</param>
        /// <returns>���� ��� ������ �����, ���������� ������ ��������/���������� ��� ���������� �����������</returns>
        [AllowAnonymous]
        [HttpPost("/confirmEmail")]
        public IActionResult ConfirmEmail([FromBody]ConfirmationViewModel confirmation)
        {
            var user = GetMemberByCode(confirmation.Email, confirmation.Code);
            if (user != null)
            {
                return Json(new { Ok = true, Result = user });
            }
            else
            {
                return Json(new { Ok = false, Message = "The confirmation code is not correct" });
            }
        }

        private HSEMember GetMemberByCode(string email, string code)
        {
            var confirmation = _context.Confirmations.Include(c => c.Member)
                .FirstOrDefault(e => e.Member.Email.ToLower() == email.ToLower().Trim());

            if (confirmation != null && confirmation.Code.ToLower() == code.Trim().ToLower())
            {
                return confirmation.Member;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ����������� ������ ������������. 
        /// </summary>
        /// <param name="model">
        /// ��� �������������, �����, ������, ������������� ������ � ���.
        /// </param>
        /// <returns>
        /// ������� �� ������ �����������, ��� ���
        /// </returns>
        [AllowAnonymous]
        [HttpPost("/register")]
        public IActionResult Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = GetMemberByCode(model.Email, model.Code);
                if (member != null)
                {
                    var user = new User
                    {
                        Name = member.Name,
                        Surname = member.Surname,
                        Patronymic = member.Patronymic,
                        Username = member.Email,
                        Password = model.Password
                    };

                    var addStatus = AddUser(user);

                    if (addStatus)
                    {
                        var confirmation = _context.Confirmations.Include(c => c.Member).FirstOrDefault(c => c.Member.Email == model.Email);
                        _context.Confirmations.Remove(confirmation);
                        AddUserToGroups(member, user);
                        _context.SaveChanges();
                        return Json(new { Ok = true, Result = true });
                    }
                    else
                        return Json(new { Ok = false, Message = "The user already exists" });
                }
                else return Json(new { Ok = false, Message = "Confirmation code is incorrect" });
            }
            else return Json(new { Ok = false, Message = "Invalid registration form" });
        }

        private bool AddUser(User user)
        {
            if (_context.Users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
                return false;
            else
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;

            }

        }

        private void AddUserToGroups(HSEMember member, User user)
        {
            if (member.MemberType == 0)
            {
                var groups = _context.Groups;

                //��������� � ����� ������ �������� 
                if (!string.IsNullOrWhiteSpace(member.Group))
                {
                    CreateOrAddToGroup(member.Group, user, member.IsGroupStarosta);
                }

                if (!string.IsNullOrWhiteSpace(member.Faculty))
                {
                    //��������� � ����� ���������� ��������
                    CreateOrAddToGroup(member.Faculty, user, member.IsFacultyStarosta);

                    //��������� � ����� ������
                    if (member.StartDate >= 1992)
                    {
                        var yearFlowGroupName = $"{member.Faculty} {member.StartDate}";
                        CreateOrAddToGroup(yearFlowGroupName, user, member.IsYearStarosta);
                    }
                }
            }
            else
            {
                //���� ���������: ���� �� ������������� ����� ��� ����������
            }
        }


        private void CreateOrAddToGroup(string groupName, User user, bool isStarosta)
        {
            if (_context.Groups.Any(g => g.Name == groupName))
            {
                var group = _context.Groups.First(g => g.Name == groupName);

                group.UserGroups.Add(new UserGroup
                {
                    GroupId = group.Id,
                    UserId = user.Id
                });
                
                if(isStarosta)
                {
                    group.Administrator = user;
                    _context.SaveChanges();
                }
            }
            else
            {
                var group = new Group
                {
                    Name = groupName,
                    Administrator = isStarosta ? user : null,
                    GroupType = GroupType.AutoCreated,
                };
                _context.Groups.Add(group);
                _context.SaveChanges();

                group.UserGroups = new List<UserGroup>
                {
                    new UserGroup
                    {
                        GroupId = group.Id,
                        UserId = user.Id
                    }
                };
            }
            _context.SaveChanges();

        }


        /// <summary>
        /// ����� ��������� ������ ����������� � ����������. ����� ��������� 10000 �����.
        /// ��������� � ���� ����� � ������ � ������� MD5, ���������� ��������� �����.
        /// ������ �� ���� �������� � ����������, ����������� [Authorized] �������� ������ � ���� �������.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/login")]
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
            // ������� JWT-�����
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

            // ������������ ������
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // ���� ������������ �� �������
            return null;
        }


    }
}