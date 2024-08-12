using CheckIN.Data.Model;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CheckIN.Services.DbContext
{
    public class DbService
    {
       private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DbService(ApplicationDbContext context, UserManager<User> userMnager)
        {
            _context = context;
            _userManager = userMnager;
        }

        public async Task<List<TitoAccount>> GetAllCustomerAccountsAsync(Guid customerId)
        {
            var accountsAndEvents = await _context.TitoAccounts
               .Include(x => x.Events)
                    .ThenInclude(x => x.Tickets)
               .Include(x => x.Events)
                     .ThenInclude(x => x.UserEvents)
                         .ThenInclude(x => x.User)
               .Where(x => x.CustomerId == customerId)
               .ToListAsync();

            return accountsAndEvents;
        }

        public async Task<UserCustomer?> GetTitoAccountsAndEventsAndTicketsCurrentUserAsync(Guid? userId)
        {
            var userCustomer = await _context.UserCustomer
                .Include(x => x.User)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)!
                        .ThenInclude(x => x.Events)
                            .ThenInclude(x => x.Tickets)                        
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return userCustomer;
        }

        public async Task<UserCustomer?> GetUserEventsForCurrentCustomerAsync(Guid? userId)
        {
            var userCustomer = await _context.UserCustomer
                .Include(x => x.User)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)!
                        .ThenInclude(x => x.Events)
                            .ThenInclude(x => x.UserEvents)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return userCustomer;
        }

        public async Task<UserCustomer?> GetAllTitoAccountUserEventsAndEventsForCurrentCustomer(Guid? userId)
        {
            var userCustomer = await _context.UserCustomer
                .Include(x => x.User)
                .Include(x => x.Customer)
                    .ThenInclude(x => x.TitoAccounts)!
                        .ThenInclude(x => x.Events)
                            .ThenInclude(x => x.UserEvents)
                                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId));

            return userCustomer;
        }

        public async Task<TitoAccount?> GetSelectedUserAccountWithEventsAsync(Guid? customerId)
        {
            var accountsAndEvents = await _context.TitoAccounts
               .Include(x => x.Events)
                .ThenInclude(x => x.UserEvents)
                    .ThenInclude(x => x.User)
                   .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsSelected);

            return accountsAndEvents;
        }

        public async Task<TitoAccount?> GetSelectedAccountWithEventsAndTicketsAsync(Guid? customerId)
        {
            var accountsAndEvents = await _context.TitoAccounts
               .Include(x => x.Events)
                    .ThenInclude(x => x.Tickets)
                    .Include(x => x.Events)
                        .ThenInclude(x => x.UserEvents)
                            .ThenInclude(x => x.User)
               .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsSelected);

            return accountsAndEvents;
        }
    }
}
