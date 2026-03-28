using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BHAS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? DepartmentID { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AuthDbConnection", throwIfV1Schema: false) { }

        public static ApplicationDbContext Create() => new ApplicationDbContext();
    }
}
