using BHAS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationUserManager UserManager
            => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        protected bool CanEditDepartment(int? entityDepartmentId)
        {
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin")) return true;
            if (!User.IsInRole("Editor")) return false;

            var userId = User.Identity.GetUserId();
            using (var db = new ApplicationDbContext())
            {
                var appUser = db.Users.Find(userId);
                return appUser != null && appUser.DepartmentID == entityDepartmentId;
            }
        }
    }
}
