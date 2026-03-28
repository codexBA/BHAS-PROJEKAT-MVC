using BHAS.Controllers;
using BHAS.DbFirst;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AS.MVCDemo.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private static readonly string[] StatusValues = { "Planiran", "Aktivan", "Završen", "Otkazan" };

        // GET: Project
        public ActionResult Index()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var projects = db.Projects
                    .Include(x => x.Department)
                    .OrderBy(x => x.ProjectName)
                    .ToList();
                return View(projects);
            }
        }

        // GET: Project/Details/5
        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var project = db.Projects
                    .Include(x => x.Department)
                    .FirstOrDefault(x => x.ProjectID == id);

                if (project == null)
                    return HttpNotFound();

                return View(project);
            }
        }

        // GET: Project/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, null);
                ViewBag.StatusList = new SelectList(StatusValues);
                return View();
            }
        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectName,DepartmentID,Budget,StartDate,EndDate,Status,Description")] Project model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    db.Projects.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, model.DepartmentID);
                ViewBag.StatusList = new SelectList(StatusValues, model.Status);
            }
            return View(model);
        }

        // GET: Project/Edit/5
        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var project = db.Projects.Find(id);
                if (project == null)
                    return HttpNotFound();

                if (!CanEditDepartment(project.DepartmentID))
                    return DepartmentAccessDenied();

                PopulateDepartmentsSelectList(db, project.DepartmentID);
                ViewBag.StatusList = new SelectList(StatusValues, project.Status);
                return View(project);
            }
        }

        // POST: Project/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectID,ProjectName,DepartmentID,Budget,StartDate,EndDate,Status,Description")] Project model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    var existing = db.Projects.Find(model.ProjectID);
                    if (existing != null && !CanEditDepartment(existing.DepartmentID))
                        return DepartmentAccessDenied();

                    model.ModifiedDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, model.DepartmentID);
                ViewBag.StatusList = new SelectList(StatusValues, model.Status);
            }
            return View(model);
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Delete(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var project = db.Projects
                    .Include(x => x.Department)
                    .FirstOrDefault(x => x.ProjectID == id);

                if (project == null)
                    return HttpNotFound();

                return View(project);
            }
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var project = db.Projects.Find(id);
                if (project == null)
                    return HttpNotFound();

                db.Projects.Remove(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        private void PopulateDepartmentsSelectList(StateStatisticsDBEntities db, int? selectedId)
        {
            var departments = db.Departments.OrderBy(x => x.DepartmentName).ToList();
            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "DepartmentName", selectedId);
        }
    }
}
