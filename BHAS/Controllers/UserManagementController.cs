using BHAS.DbFirst;
using BHAS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserManagementController : BaseController
    {
        public ActionResult Index()
        {
            using (var authDb = new ApplicationDbContext())
            using (var appDb = new StateStatisticsDBEntities())
            {
                var departments = appDb.Departments
                    .ToDictionary(d => d.DepartmentID, d => d.DepartmentName);

                var allRoles = authDb.Roles.ToDictionary(r => r.Id, r => r.Name);
                var users = authDb.Users.Include("Roles").ToList();

                var list = users.Select(u => new UserListItem
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Roles = u.Roles.Select(r => allRoles[r.RoleId]).ToList(),
                    DepartmentName = u.DepartmentID.HasValue
                        ? (departments.ContainsKey(u.DepartmentID.Value)
                            ? departments[u.DepartmentID.Value] : null)
                        : null
                }).ToList();

                return View(list);
            }
        }

        public ActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                DepartmentID = model.DepartmentID,
                EmailConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, model.Role);
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            PopulateDropdowns();
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            var roles = await UserManager.GetRolesAsync(id);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                DepartmentID = user.DepartmentID
            };

            PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null) return HttpNotFound();

            user.FullName = model.FullName;
            user.DepartmentID = model.DepartmentID;
            await UserManager.UpdateAsync(user);

            var currentRoles = await UserManager.GetRolesAsync(user.Id);
            await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
            await UserManager.AddToRoleAsync(user.Id, model.Role);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (id == User.Identity.GetUserId())
            {
                TempData["Error"] = "Ne možeš obrisati vlastiti nalog.";
                return RedirectToAction("Index");
            }

            using (var authDb = new ApplicationDbContext())
            {
                var superAdminRole = authDb.Roles.FirstOrDefault(r => r.Name == "SuperAdmin");
                if (superAdminRole != null)
                {
                    int superAdminCount = authDb.Users
                        .Count(u => u.Roles.Any(r => r.RoleId == superAdminRole.Id));
                    bool isTarget = authDb.Users
                        .Any(u => u.Id == id && u.Roles.Any(r => r.RoleId == superAdminRole.Id));

                    if (isTarget && superAdminCount <= 1)
                    {
                        TempData["Error"] = "Zadnji SuperAdmin ne može biti obrisan.";
                        return RedirectToAction("Index");
                    }
                }

                var user = authDb.Users.Find(id);
                if (user == null) return HttpNotFound();

                user.Roles.Clear();
                authDb.Users.Remove(user);
                authDb.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private void PopulateDropdowns()
        {
            ViewBag.Roles = new[] { "SuperAdmin", "Admin", "Editor", "User" };

            using (var appDb = new StateStatisticsDBEntities())
            {
                ViewBag.Departments = appDb.Departments
                    .OrderBy(d => d.DepartmentName)
                    .ToList();
            }
        }
    }
}
