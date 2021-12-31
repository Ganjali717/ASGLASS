using ASGlass.DAL;
using ASGlass.Models;
using ASGlass.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace ASGlass.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IHubContext<ASGlassHub> _hubContext;


        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context, IHubContext<ASGlassHub> hubContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            AppUser member = new AppUser
            {
                UserName = "Ganjali717",
                FullName = "Ganjali Imanov"
            };

            var result = await _userManager.CreateAsync(member, "Genceli717");



            return View(result);
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View();


            AppUser member = _userManager.Users.FirstOrDefault(x => x.UserName == loginVM.UserName && x.IsAdmin == false);

            if (member == null)
            {
                ModelState.AddModelError("", "istifadeci adi ve ya sifre yanlisdir!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(member, loginVM.Password, loginVM.IsPersistent, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "istifadeci adi ve ya sifre yanlisdir!");
                return View();
            }

            await _hubContext.Clients.All.SendAsync("Login");


            return RedirectToAction("index", "home");

        }

        public async Task<IActionResult> Logout()
        {
           
            await _signInManager.SignOutAsync();
            await _hubContext.Clients.All.SendAsync("Logout");

            return RedirectToAction("index", "home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser member = await _userManager.FindByNameAsync(registerVM.UserName);

            if (member != null)
            {
                ModelState.AddModelError("UserName", "Bu ad istifadə olunub!");
                return View();
            }

            member = await _userManager.FindByEmailAsync(registerVM.Email);
            if (member != null)
            {
                ModelState.AddModelError("Email", "Bu Email ünvanı istifadə olunub!");
                return View();
            }

            member = new AppUser
            {
                FullName = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Phone = registerVM.Phone,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(member, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            var roleResult = await _userManager.AddToRoleAsync(member, "Member");
            await _signInManager.SignInAsync(member, true);

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> ShowAccount(string id)
        {
            AppUser appUsers = _context.AppUsers.Include(x => x.Orders).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == id);
            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            if (appUsers == null || appUsers.Id != id) return RedirectToAction("index", "error");

            ProfileViewModel passwordVM = new ProfileViewModel
            {
                Email = member.Email,
                FullName = member.FullName,
                PhoneNumber = member.PhoneNumber,
                UserName = member.UserName,
                appUser = appUsers
            };

            return View(passwordVM);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowAccount(ProfileViewModel profileVM)
        {
            TempData["Success"] = false;
           

            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!string.IsNullOrWhiteSpace(profileVM.ConfirmPassword) && !string.IsNullOrWhiteSpace(profileVM.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(member, profileVM.OldPassword, profileVM.NewPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var item in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                    return View();
                }

            }

            if (member.Email != profileVM.Email && _userManager.Users.Any(x => x.NormalizedEmail == profileVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Bu Email adresi istifadə olunub!");
                return View();
            }

            member.FullName = profileVM.FullName;
            member.Email = profileVM.Email;
            member.PhoneNumber = profileVM.PhoneNumber;
            member.UserName = profileVM.UserName;

            var result = await _userManager.UpdateAsync(member);

            await _signInManager.SignInAsync(member, true);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View();
            }

            TempData["Success"] = true;

            return RedirectToAction("showaccount");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new MailMessage();
            message.To.Add(new MailAddress("anar.aliyev717@gmail.com"));
            message.From = new MailAddress(user.Email);
            message.Subject = "Reset Password Token";
            message.Body = callback;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "anar.aliyev717@gmail.com",
                    Password = "genceli717"
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
