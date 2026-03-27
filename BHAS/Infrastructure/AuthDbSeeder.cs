using BHAS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BHAS.Infrastructure
{
    public static class AuthDbSeeder
    {
        public static void Seed()
        {
            using (var db = new ApplicationDbContext())
            {
                db.Database.CreateIfNotExists();

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                string[] roles = { "SuperAdmin", "Admin", "Editor", "User" };
                foreach (var role in roles)
                    if (!roleManager.RoleExists(role))
                        roleManager.Create(new IdentityRole(role));

                if (userManager.FindByEmail("admin@bhas.ba") == null)
                {
                    var superAdmin = new ApplicationUser
                    {
                        UserName = "admin@bhas.ba",
                        Email = "admin@bhas.ba",
                        FullName = "Super Administrator",
                        EmailConfirmed = true
                    };
                    var result = userManager.Create(superAdmin, "Admin123");
                    if (result.Succeeded)
                        userManager.AddToRole(superAdmin.Id, "SuperAdmin");
                }
            }
        }
    }
}
