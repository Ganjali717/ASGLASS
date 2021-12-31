using ASGlass.Areas.Manage.ViewModels;
using ASGlass.DAL;
using ASGlass.Models;
using ASGlass.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.Areas.Manage.Controllers
{
    [Area("manage")]

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Index()
        {
            /* AppUser admin = new AppUser
             {
                 UserName = "SuperAdmin",
                 FullName = "Ali Imanov"
             };

             var result = await _userManager.CreateAsync(admin, "Admin123");

             List<string> errors = new List<string>();
             if (!result.Succeeded)
             {
                 foreach (var item in result.Errors)
                 {
                     errors.Add(item.Description);
                 }

                 return Ok(errors);
             }*/

            /*AppUser appUser = await _userManager.FindByNameAsync("SuperAdmin");
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");*/

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser admin = _userManager.Users.FirstOrDefault(x => x.UserName == loginVM.UserName && x.IsAdmin == true);

            if (admin == null)
            {
                ModelState.AddModelError(loginVM.UserName, "istifadeci adi ve ya sifre yanlisdir!");
            }

            if(loginVM.Password == null)
            {
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(admin, loginVM.Password, loginVM.IsPersistent, false);


            if (!result.Succeeded)
            {
                ModelState.AddModelError(loginVM.Password, "istifadeci adi ve ya sifre yanlisdir!");
            }

            return RedirectToAction("index", "dashboard");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }


        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AddAdmin()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddAdmin(AdminRegisterViewModel adminRegisterVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser admin = await _userManager.FindByNameAsync(adminRegisterVM.UserName);
            if (admin != null)
            {
                ModelState.AddModelError("UserName", "UserName already taken!");
                return View();
            }

            admin = await _userManager.FindByEmailAsync(adminRegisterVM.Email);
            if (admin != null)
            {
                ModelState.AddModelError("Email", "Email already taken!");
                return View();
            }


            admin = new AppUser
            {
                FullName = adminRegisterVM.FullName,
                UserName = adminRegisterVM.UserName,
                Email = adminRegisterVM.Email,
                IsAdmin = true
            };

            var result = await _userManager.CreateAsync(admin, adminRegisterVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");
            await _signInManager.SignInAsync(admin, true);

            return RedirectToAction("index", "dashboard");
        }


        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileViewModel passwordVM = new ProfileViewModel
            {
                appUser = appUser
            };
            return View(passwordVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel profileVM)
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
    }
}
