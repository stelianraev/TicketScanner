using CheckIN.Data.Model;
using CheckIN.Models.TITo;
using CheckIN.Models.ViewModels;
using CheckIN.Services;
using CheckIN.Services.Cache;
using CheckIN.Services.Customer;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CheckIN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerProvider _customerProvider;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly Common _commonHelper;
        private readonly ICache _cache;

        public AccountController(
            ICache chace,
            ApplicationDbContext context,
            Common commonHelper,
            ICustomerProvider customerProvider,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _context = context;
            _customerProvider = customerProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonHelper = commonHelper;
            _cache = chace;
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
                    UserCustomerCache? userCustomerCache = null;

                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains(Permission.Owner.ToString()))
                    {
                        if (!_cache.Contains("Customers"))
                        {
                            _cache.Add("Customers", new List<UserCustomerCache>());
                        }
                        else
                        {
                            var allUserCustomers = _cache.GetData<List<UserCustomerCache>>("Customers");
                            var currentCustomer = allUserCustomers.FirstOrDefault(x => x.UserId == user.Id);

                            if (currentCustomer == null)
                            {
                                currentCustomer = new UserCustomerCache()
                                {
                                    UserId = user.Id,
                                    CustomerId = user.UserCustomers?.FirstOrDefault()?.CustomerId,
                                    CustomerName = "Test"
                                };

                                allUserCustomers.Add(currentCustomer);
                            }
                        }

                        var customer = await _context.UserCustomer
                            .Include(x => x.Customer)
                            .FirstOrDefaultAsync(x => x.UserId == user.Id);

                        if (customer.Customer.TitoToken != null)
                        {
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
        [Authorize(Roles = "Admin")]
        public IActionResult UserRegistration()
        {
            var customerId = GetCurrentCustomer();

            if (customerId == Guid.Empty)
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
        public async Task<IActionResult> UserRegistration(UsersFormModel users)
        {
            var customerId = GetCurrentCustomer();

            if (customerId == Guid.Empty)
            {
                return Unauthorized();
            }

            //if (users.NewUser.Password != users.NewUser.ConfirmPassword)
            //{
            //    ModelState.AddModelError(users.NewUser.Password, "Passwords does not match");
            //}

            var customer = await _context.UserCustomer.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer?.Customer == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid customer.");
            }

            //var existingUser = _context.Users.FirstOrDefault(x => x.Email == users.NewUser.Email);
            var existingUser = _context.Users.FirstOrDefault(x => x.Id == customer.UserId);

            if (existingUser != null)
            {
                ModelState.AddModelError("Users", "This email is already used with this customer");
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
                UserCustomers = new List<UserCustomer>()
            };

            newUser.UserCustomers.Add(customer);

            var result = await _userManager.CreateAsync(newUser, users.NewUser.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(users.NewUser.Permission.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>() { Name = users.NewUser.Permission.ToString() });
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
            if (customer.Password != customer.ConfirmPassowrd)
            {
                ModelState.AddModelError(customer.Password, "Passwords do not match");
            }

            var user = _context.Users.FirstOrDefault(x => x.Email == customer.Email);
            //var doesCustomerExist = _context.Customers.Any(x => x.Email == customer.Email && x.Name == customer.Name);

            if (user != null)
            {
                ModelState.AddModelError(customer.Email, "Customer with such email address or name already exists");
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            user = new User
            {
                UserName = customer.Email,
                Email = customer.Email,
                Permision = Permission.Owner
            };

            var userCustomer = new UserCustomer()
            {
                User = user,
                Owner = user
            };

            var newCustomer = new Customer()
            {
                Name = customer.Name,
                UserCustomers = new List<UserCustomer>() { userCustomer },
                TitoToken = null
            };

            await _context.Customers.AddAsync(newCustomer);
            var result = await _userManager.CreateAsync(user, customer.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Permission.Owner.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>() { Name = Permission.Owner.ToString() });
                }

                await _userManager.AddToRoleAsync(user, Permission.Owner.ToString());

                //await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(customer);
        }

        private Guid GetCurrentCustomer()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            var userCustomer = user.UserCustomers.FirstOrDefault(x => x.UserId == user.Id);
            return userCustomer.CustomerId;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            var userViewModel = new UserFormModel();
            userViewModel.Email = user.Email;
            userViewModel.Permission = user.Permision;

            return this.View(userViewModel);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(UserFormModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(userModel);
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == userModel.Id);

            if (user == null)
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
