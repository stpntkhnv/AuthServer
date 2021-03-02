using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.ViewModel;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index(string returnUrl)
        {
            var model = CreateAuthViewModel(returnUrl);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return BadRequest("User not found");
            }

            if (model.Password == user.Password)
            {
                var issuer = new IdentityServerUser(user.Id)
                {
                    DisplayName = user.UserName
                };
                await HttpContext.SignInAsync(issuer);
                return Redirect(model.ReturnUrl);
            }

            return BadRequest("incorrect parol");
        }

        //TODO Refactor all 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.ReturnUrl is null)
            {
                model.ReturnUrl = "asdasd";
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                return BadRequest("asd");
                //return View("Index", model);
            }

            User user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                Password = model.Password
            };

            var registerResult = await _userManager.CreateAsync(user, model.Password);

            if (!registerResult.Succeeded)
            {
                ModelState.AddModelError("", "User dont registered");
                return BadRequest("asd");
                //return View("Index", model);
            }

            user = await _userManager.FindByEmailAsync(model.Email);

            var issuer = new IdentityServerUser(user.Id)
            {
                DisplayName = user.UserName
            };


            await HttpContext.SignInAsync(issuer);
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public AuthViewModel CreateAuthViewModel(string returnUrl)
        {
            AuthViewModel model = new AuthViewModel
            {
                ReturnUrl = returnUrl
            };

            return model;
        }

        public AuthViewModel CreateAuthViewModelFromRegisterViewModel(RegisterViewModel model)
        {
            AuthViewModel vm = new AuthViewModel
            {
                ReturnUrl = model.ReturnUrl,
                Email = model.Email,
                Password = model.Password
            };

            return vm;
        }
    }
}