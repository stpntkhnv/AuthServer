using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AuthServer.Data;
using AuthServer.Models;
using AuthServer.ViewModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ApplicationContext _context;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IIdentityServerInteractionService interaction, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _context = context;
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

            UserProfile profile = new UserProfile
            {
                Email = user.Email,
                UserName = user.UserName,
                AvatarUrl = "https://www.freepngimg.com/thumb/facebook/62681-flat-icons-face-computer-design-avatar-icon.png",
                FirstName = model.FirstName,
                LastName = model.LastName,
                Id = Guid.NewGuid().ToString(),
                Status = string.Empty,
                UserId = user.Id
            };

            var addProfileResult = await _context.AddAsync(profile);
            await _context.SaveChangesAsync();

            user = await _userManager.FindByEmailAsync(model.Email);

            var issuer = new IdentityServerUser(user.Id)
            {
                DisplayName = user.UserName
            };


            await HttpContext.SignInAsync(issuer);
            
            return RedirectToAction("Index", "Home");
        }

        //TODO added return to cli4nt's returnUrl
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var a = HttpContext;
            var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);
            string returnUrl = logoutContext.PostLogoutRedirectUri;
            await HttpContext.SignOutAsync();
            return Redirect("http://localhost:3000");
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