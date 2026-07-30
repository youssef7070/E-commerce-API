using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Seeding
{
    public class IdentityDataSeeding : IDataSeeder
    {
        private readonly StoreIdentityDbContexts _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataSeeding> _logger;

        public IdentityDataSeeding(StoreIdentityDbContexts dbContext , UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager , ILogger<IdentityDataSeeding> logger)
        {

           _dbContext = dbContext;
           _userManager = userManager;
           _roleManager = roleManager;
           _logger = logger;

        }



        public async Task SeedAsync(CancellationToken ct = default)
        {
            
            try
            {

                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(ct);

                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync(ct);
                }

                if (!await _roleManager.Roles.AnyAsync(ct))
                {

                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

                }

                if (!await _userManager.Users.AnyAsync(ct))
                {
                    var Admin = new ApplicationUser
                    {
                        UserName = "superadmin",
                        Email = "superadmin@example.com",
                        DisplayName = "Super Admin"
                    };
                    }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }   
        }


    }
}
