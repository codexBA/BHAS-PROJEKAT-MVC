using BHAS.DbFirst;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    [Authorize]
    public class DepartmentController : BaseController
    {
        public ActionResult Index()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                return View(db.Departments.ToList());
            }
        }

        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var dpt = db.Departments
                    .Include(x => x.Employee)
                    .Include(x => x.Employees)
                    .FirstOrDefault(x => x.DepartmentID == id);

                if (dpt == null)
                    return HttpNotFound();

                return View(dpt);
            }
        }

        // GET: Department/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateManagersSelectList(db, null);
                return View();
            }
        }

        // POST: Department/Create
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentName,DepartmentCode,Budget,ManagerID")] Department model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    db.Departments.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateManagersSelectList(db, model.ManagerID);
            }
            return View(model);
        }

        // GET: Department/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var department = db.Departments.Find(id);
                if (department == null)
                    return HttpNotFound();

                PopulateManagersSelectList(db, department.ManagerID);
                return View(department);
            }
        }

        // POST: Department/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentID,DepartmentName,DepartmentCode,Budget,ManagerID")] Department model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.ModifiedDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateManagersSelectList(db, model.ManagerID);
            }
            return View(model);
        }

        // GET: Department/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var department = db.Departments.Find(id);
                if (department == null)
                    return HttpNotFound();

                return View(department);
            }
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                try
                {
                    var department = db.Departments.Find(id);
                    if (department == null)
                        return HttpNotFound();

                    db.Departments.Remove(department);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["Error"] = "Nije moguće obrisati odjel jer ima zaposlenih. Prvo premjestite ili obrišite zaposlene.";
                    return RedirectToAction("Details", new { id });
                }
            }
        }

        // GET: Department/EmployeesPartial?departmentId=5
        [HttpGet]
        public ActionResult EmployeesPartial(int departmentId)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var employees = db.Employees
                    .Where(x => x.DepartmentID == departmentId)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList();

                return PartialView("_EmployeesTable", employees);
            }
        }

        private void PopulateManagersSelectList(StateStatisticsDBEntities db, int? selectedEmployeeId)
        {
            var employees = db.Employees
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    x.EmployeeID,
                    FullName = x.LastName + " " + x.FirstName
                })
                .ToList();

            ViewBag.ManagerID = new SelectList(employees, "EmployeeID", "FullName", selectedEmployeeId);
        }
    }
}
