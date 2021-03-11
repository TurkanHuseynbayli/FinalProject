using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Extensions;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password wrong!");
                return View();
            }
            if (user.isDelete)
            {
                ModelState.AddModelError("", "This account blocked!");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult inResult =
                await _signInManager.PasswordSignInAsync(user, login.Password, true, true);

            if (inResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Try after a few minutes");
                return View(login);
            }

            if (!inResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password wrong!!!");
                return View(login);
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            AppUser newUser = new AppUser()
            {
                Name = register.Name,
                Surname = register.Surname,
                UserName = register.Username,
                Email = register.Email
            };

            IdentityResult identityResult = await _userManager.CreateAsync(newUser, register.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(newUser, Roles.Member.ToString());
            await _signInManager.SignInAsync(newUser, true);


            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #region Create User Roles
        //public async Task CreateUserRole()
        //{
        //    if (!(await _roleManager.RoleExistsAsync(Roles.Admin.ToString())))
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin.ToString() });
        //    if (!(await _roleManager.RoleExistsAsync(Roles.Member.ToString())))
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Member.ToString() });
        //}
        #endregion


        public IActionResult Subscribe()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(SubscribedEmail subscribedEmail)
        {
            if (ModelState.IsValid)
            {
                SubscribedEmail subscribed = new SubscribedEmail();
                subscribed.Email = subscribedEmail.Email.Trim().ToLower();
                bool isExist = _context.SubscribedEmails
                      .Any(e => e.Email.Trim().ToLower() == subscribedEmail.Email.Trim().ToLower());
                if (isExist)
                {
                    ModelState.AddModelError("", "This email already subscribed");
                }
                else
                {
                    await _context.SubscribedEmails.AddAsync(subscribed);
                    await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage(CommentVM comment)
        {
            if (ModelState.IsValid)
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = comment.Email,
                        Password = comment.Password
                    }
                };
                MailAddress fromEmail = new MailAddress(comment.Email, comment.Name);
                MailAddress toEmail = new MailAddress("turkanhuseynbayli@gmail.com", comment.Name);
                MailMessage message = new MailMessage()
                {
                    From = fromEmail,
                    Body = comment.Message
                };
                message.To.Add(toEmail);
                client.Send(message);
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LeaveComment(CommentVM model)
        {
            JsonResult result = new JsonResult(0);
           // try
           // {
           //     var comment = new Comment();
           //     comment.Text = model.Text;
           //     comment.TimeStamp = DateTime.Now;

           //     var res=await _context.Comments.AddAsync(comment);
           //     await _context.SaveChangesAsync();

           //     result.Data = new { Success = res };
           // }
           //catch (Exception ex)
           // {
           //     result.Data = new { Success = false, SendMessage = ex.Message };
           // }

            
            return result;
        }

    }
}
