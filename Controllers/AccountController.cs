using CheckIN.Data.Model;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
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
        private readonly Common _commonHelper;

        public AccountController(ApplicationDbContext context, Common commonHelper, ICustomerProvider customerProvider, SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _customerProvider = customerProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonHelper = commonHelper;
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

                    if (roles.Contains(Permission.Admin.ToString()))
                    {
                        var token = _context.CustomerSettings.FirstOrDefault(x => x.CustomerId == user.CustomerId);

                        if(token.TitoToken != null)
                        {
                            //TitoSettings settings = new TitoSettings();
                            //settings.Token = token.TitoToken;

                            //return RedirectToAction("LoginWith", "Admin", settings);
                            return RedirectToAction("AdminSettings", "Admin");
                        }

                        return RedirectToAction("LoginWith", "Admin");
                    }
                    else if (roles.Contains(Permission.Checker.ToString()))
                    {
                        return RedirectToAction("Index", "CheckIn");
                    }
                    else if (roles.Contains(Permission.Scanner.ToString()))
                    {

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

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult UserRegistration()
        {
            var customerId = GetCurrentCustomer();

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserRegistration(UsersViewModel users)
        {
            var customerId = GetCurrentCustomer();

            if (String.IsNullOrEmpty(customerId) || String.IsNullOrWhiteSpace(customerId))
            {
                return Unauthorized();
            }

            //if (users.NewUser.Password != users.NewUser.ConfirmPassword)
            //{
            //    ModelState.AddModelError(users.NewUser.Password, "Passwords does not match");
            //}

            var iSUserExist = _context.Users.FirstOrDefault(x => x.Email == users.NewUser.Email);

            if (iSUserExist != null)
            {
                ModelState.AddModelError("Users", "This email is already used");
            }                      

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid customer.");
            }

            if (!ModelState.IsValid)
            {
                return View(users);
            }

            var newUser = new User
            {
                UserName = users.NewUser.Email,
                Email = users.NewUser.Email,
                Permision = users.NewUser.Permission,
                CustomerId = customerId,
            };

            var result = await _userManager.CreateAsync(newUser, users.NewUser.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(users.NewUser.Permission.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(users.NewUser.Permission.ToString()));
                }

                await _userManager.AddToRoleAsync(newUser, users.NewUser.Permission.ToString());

                await _context.SaveChangesAsync();
                return RedirectToAction("Users", "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(users.NewUser);
            }
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
                CustomerId = newCustomer.CanonicalId,
                Permision = Permission.Admin
            };

            var result = await _userManager.CreateAsync(adminUser, customer.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Permission.Admin.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Permission.Admin.ToString()));
                }

                await _userManager.AddToRoleAsync(adminUser, Permission.Admin.ToString());

                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(customer);
        }

        private string GetCurrentCustomer()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            return user.CustomerId;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            var userViewModel = new UserViewModel();
            userViewModel.Email = user.Email;
            userViewModel.Permission = user.Permision;

            return this.View(userViewModel);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(UserViewModel userModel)
        {
            if(!ModelState.IsValid)
            {
                return this.View(userModel);
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == userModel.Id);

            if(user == null)
            {
                return this.View(userModel);
            }

            user.Email = userModel.Email;

            user.PasswordHash = userModel.Password != null ? _commonHelper.PasswordHash(userModel.Password) : user.PasswordHash;
            //user.PasswordHash = _commonHelper.PasswordHash(userModel.Password);
            user.Permision = userModel.Permission;

            await _context.SaveChangesAsync();

            return RedirectToAction("Users", "Admin");
        }
    }
}
