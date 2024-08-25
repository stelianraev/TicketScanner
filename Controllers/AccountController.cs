﻿using CheckIN.Common;
using CheckIN.Data.Model;
using CheckIN.Models.ViewModels;
using CheckIN.Services.DbContext;
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
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly PasswordHashingService _passwordHashingService;
        private readonly DbService _dbService;
        //private readonly ICache _cache;

        public AccountController(
            /*ICache chace*/
            DbService dbService,
            PasswordHashingService passwordHashingService,
            ApplicationDbContext context,
            //ICustomerProvider customerProvider,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHashingService = passwordHashingService;
            _dbService = dbService;
            //_cache = chace;
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
            try
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
                        //UserCustomerCache? userCustomerCache = null;

                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains(Permission.Owner.ToString()))
                        {
                            //if (!_cache.Contains("Customers"))
                            //{
                            //    _cache.Add("Customers", new List<UserCustomerCache>());
                            //}
                            //else
                            //{
                            //    var allUserCustomers = _cache.GetData<List<UserCustomerCache>>("Customers");
                            //    var currentCustomer = allUserCustomers.FirstOrDefault(x => x.UserId == user.Id);

                            //    if (currentCustomer == null)
                            //    {
                            //        currentCustomer = new UserCustomerCache()
                            //        {
                            //            UserId = user.Id,
                            //            CustomerId = user.UserCustomers?.FirstOrDefault()?.CustomerId,
                            //            CustomerName = "Test"
                            //        };

                            //        allUserCustomers.Add(currentCustomer);
                            //    }
                            //}

                            //var userCustomer = await _dbService.GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(user.Id);
                            var userCustomer = await _context.UserCustomer
                                .Include(x => x.User)
                                .Include(x => x.Customer)
                                .FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));

                            //var userCustomer = await _context.UserCustomer
                            //    .Include(x => x.User)
                            //    .Include(x => x.Customer)
                            //    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                            if (userCustomer == null) 
                            {
                                ModelState.AddModelError("", "The user is not exist");
                            }
                            else
                            {
                                if (userCustomer.Customer.TitoToken != null)
                                {
                                    return RedirectToAction("AdminSettings", "Admin");
                                }
                                else
                                {
                                    return RedirectToAction("LoginWith", "Admin");
                                }                                
                            }                            
                        }
                        else if (roles.Contains(Permission.Checker.ToString()))
                        {
                            return RedirectToAction("Index", "CheckIn");
                        }
                        else if (roles.Contains(Permission.Scanner.ToString()))
                        {

                        };
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            catch(Exception ex)
            {
                //Log Exception
            }

            return this.View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> UserRegistration()
        {
            //var customerId = await GetCurrentCustomerAsync();
            var user = await _userManager.GetUserAsync(User);
            var userCustomer = await _dbService.GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(user?.Id);

            if (userCustomer?.CustomerId == Guid.Empty)
            {
                return Unauthorized();
            }

            var model = new UserRegistrationViewModel
            {
                CustomerId = userCustomer.CustomerId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> UserRegistration(UsersFormModel users)
        {
            var user = await _userManager.GetUserAsync(User);
            var userCustomer = await _dbService.GetUserEventsForCurrentCustomerAsync(user?.Id);

            if (userCustomer?.CustomerId == Guid.Empty)
            {
                return Unauthorized();
            }

            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == users.NewUser.Email);

            var selectedAccount = userCustomer?.Customer?.TitoAccounts?.FirstOrDefault(x => x.IsSelected);
            var selectedEvent = selectedAccount?.Events?.FirstOrDefault(x => x.IsSelected);

            //TODO
            if (_context.Entry(selectedEvent).State == EntityState.Detached)
            {
                _context.Attach(selectedEvent);
            }

            if (existingUser != null)
            {
                var existingUserInEvent = selectedEvent?.UserEvents?.FirstOrDefault(x => x.UserId == existingUser.Id);

                if (existingUserInEvent != null)
                {
                    ModelState.AddModelError("Users", "This email is already used with this customer");
                    return View(users.NewUser);
                }

                var newUserEvent = new UserEvent()
                {
                    Event = selectedEvent,
                    UserId = existingUser.Id
                };

                selectedEvent!.UserEvents.Add(newUserEvent);

                await _context.SaveChangesAsync();

                return RedirectToAction("Users", "Admin");
            }

            var newUser = new User
            {
                UserName = users.NewUser.Email,
                Email = users.NewUser.Email,
                Permision = users.NewUser.Permission
            };

            var result = await _userManager.CreateAsync(newUser, users.NewUser.Password);

            if (result.Succeeded)
            {
                var newUserEvent = new UserEvent()
                {
                    Event = selectedEvent,
                    UserId = newUser.Id
                };

                var newUserCustomer = new UserCustomer()
                {
                    Customer = userCustomer!.Customer,
                    UserId = newUser.Id,
                    Owner = userCustomer.Owner
                };

                //await _context.UserCustomer.AddAsync(newUserCustomer);

                selectedEvent!.UserEvents.Add(newUserEvent);

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
                Permision = Permission.Owner,
                UserCustomers = new List<UserCustomer>()
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
                Email = customer.Email,
                TitoToken = null
            };

            userCustomer.Customer = newCustomer;

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

        //private async Task<Guid> GetCurrentCustomerAsync()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var userCustomer = await _context.UserCustomer.FirstOrDefaultAsync(x => x.UserId.ToString() == userId);
        //    var customer = userCustomer.Customer;
        //    return userCustomer.CustomerId;
        //}

        //private async Task<UserCustomer> GetCurrentUserCustomerAsync()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var userCustomer = await _context.UserCustomer
        //        .Include(x => x.Customer)
        //        .FirstOrDefaultAsync(x => x.UserId.ToString() == userId);

        //    return userCustomer;
        //}

        [HttpGet]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> EditUser(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            var userViewModel = new UserFormModel();
            userViewModel.Email = user.Email;
            userViewModel.Permission = user.Permision;

            return this.View(userViewModel);

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner")]
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

            user.PasswordHash = userModel.Password != null ? _passwordHashingService.PasswordHash(userModel.Password) : user.PasswordHash;
            //user.PasswordHash = _commonHelper.PasswordHash(userModel.Password);
            user.Permision = userModel.Permission;

            await _context.SaveChangesAsync();

            return RedirectToAction("Users", "Admin");
        }
    }
}
