using CheckIN.Data.Model;
using CheckIN.Models.ViewModels;
using CheckIN.Services.Customer;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext context, ICustomerProvider customerProvider, SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _customerProvider = customerProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Entry()
        {            
            return View();
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

                var result = await _signInManager.PasswordSignInAsync(user, model.Password!, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("UserDashboard", "User");
                    }
                    // Add more role checks and redirects as necessary

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return this.View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserRegistration()
        {
            var customerId = GetCurrentTenantId();

            if (String.IsNullOrEmpty(customerId) || String.IsNullOrWhiteSpace(customerId))
            {
                return Unauthorized();
            }

            var model = new UserRegistrationViewModel
            {
                CustomerId = customerId
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserRegistration(UserRegistrationViewModel user)
        {
            var customerId = GetCurrentTenantId();

            if (String.IsNullOrEmpty(customerId) || String.IsNullOrWhiteSpace(customerId))
            {
                return Unauthorized();
            }

            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError(user.Password, "Passwords does not match");
            }

            if(!ModelState.IsValid)
            {
                return View(user);
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid customer.");

                return View(user);
            }

            var newUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                CustomerId = user.CustomerId
            };


          var result = await _userManager.CreateAsync(newUser, user.Password);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(user);
            }

            return RedirectToAction("Index", "Home"); 
        }

        [HttpGet]
        public IActionResult CustomerRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRegistration(CustomerRegistrationViewModel customer)
        {
            if(customer.Password != customer.ConfirmPassowrd)
            {
                ModelState.AddModelError(customer.Password, "Passwords does not match");
                return View(customer);
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var customerCanonicalId = Guid.NewGuid().ToString();
            var newCustomer = new Customer { CanonicalId = customerCanonicalId,  Name = customer.Name };
            _context.Customers.Add(newCustomer);           

            var adminUser = new User
            {
                UserName = customer.Email,
                Email = customer.Email,
                CustomerId = newCustomer.CanonicalId
            };

            var result = await _userManager.CreateAsync(adminUser, customer.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await _userManager.AddToRoleAsync(adminUser, "Admin");

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(customer);
        }

        private string GetCurrentTenantId()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            return user.CustomerId;
        }
    }
}
