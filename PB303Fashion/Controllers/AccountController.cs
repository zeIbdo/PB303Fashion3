using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PB303Fashion.DataAccessLayer;
using PB303Fashion.DataAccessLayer.Entities;
using PB303Fashion.Models;

namespace PB303Fashion.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm,string returnUrl=null)
        {
            if (!ModelState.IsValid) return View(vm);
            var userByName = await _userManager.FindByNameAsync(vm.Username);
            if ( userByName == null)
            {
                ModelState.AddModelError("", "Username or password are incorrect");
                return View(vm);
            }
            var result = await _signInManager.PasswordSignInAsync(userByName, vm.Password, true, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password are incorrect");
                return View(vm);
            }
            

            return Redirect(returnUrl);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.FindByNameAsync(vm.Username);

            if (user != null)
            {
                ModelState.AddModelError("", "User with this name already exists!");

                return View(vm);
            }
            
            var newUser = new AppUser
            {
                UserName = vm.Username,
                Email = vm.Email
            };
            var result = await _userManager.CreateAsync(newUser, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description); 
                    return View(vm);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
