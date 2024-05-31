using CheckIN.Data.Model;
using CheckIN.Models.ViewModels;
using CheckIN.Services.Customer;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckIN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerProvider _customerProvider;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(ApplicationDbContext context, ICustomerProvider customerProvider, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _customerProvider = customerProvider;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == model.Username || u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }




        public IActionResult Registration() => View();

        [HttpPost]
        public async Task<IActionResult> UserRegistration(LoginViewModel user)
        {
            if(ModelState.IsValid)
            {
                return View(user);
            }

            var registeredUser = new User
            {                
                Email = user.Email
                
            };

          var result = await _userManager.CreateAsync(registeredUser, user.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                foreach(var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(user);
            }

            return RedirectToAction("Registration", "Users"); 
        }
    }
}
