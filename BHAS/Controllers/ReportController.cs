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
    public class ReportController : BaseController
    {
        private static readonly string[] StatusValues = { "Draft", "U obradi", "Završen", "Arhiviran" };
        private static readonly string[] ReportTypeValues = { "Statistički", "Finansijski", "Analitički", "Operativni" };

        // GET: Report
        public ActionResult Index()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var reports = db.Reports
                    .Include(x => x.Department)
                    .Include(x => x.Employee)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();
                return View(reports);
            }
        }

        // GET: Report/Details/5
        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var report = db.Reports
                    .Include(x => x.Department)
                    .Include(x => x.Employee)
                    .Include(x => x.ReportDatas.Select(rd => rd.Region))
                    .FirstOrDefault(x => x.ReportID == id);

                if (report == null)
                    return HttpNotFound();

                return View(report);
            }
        }

        // GET: Report/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, null);
                PopulateEmployeesSelectList(db, null);
                ViewBag.StatusList = new SelectList(StatusValues);
                ViewBag.ReportTypeList = new SelectList(ReportTypeValues);
                return View();
            }
        }

        // POST: Report/Create
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportName,DepartmentID,CreatedBy,Status,ReportType,Description")] Report model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.CreatedDate = DateTime.Now;
                    db.Reports.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = model.ReportID });
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, model.DepartmentID);
                PopulateEmployeesSelectList(db, model.CreatedBy);
                ViewBag.StatusList = new SelectList(StatusValues, model.Status);
                ViewBag.ReportTypeList = new SelectList(ReportTypeValues, model.ReportType);
            }
            return View(model);
        }

        // GET: Report/Edit/5
        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var report = db.Reports.Find(id);
                if (report == null)
                    return HttpNotFound();

                if (!CanEditDepartment(report.DepartmentID))
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden,
                        "Nemaš pristup izvještajima iz ovog odjela.");

                PopulateDepartmentsSelectList(db, report.DepartmentID);
                PopulateEmployeesSelectList(db, report.CreatedBy);
                ViewBag.StatusList = new SelectList(StatusValues, report.Status);
                ViewBag.ReportTypeList = new SelectList(ReportTypeValues, report.ReportType);
                return View(report);
            }
        }

        // POST: Report/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReportID,ReportName,DepartmentID,CreatedBy,Status,ReportType,Description")] Report model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    var existing = db.Reports.Find(model.ReportID);
                    if (existing != null && !CanEditDepartment(existing.DepartmentID))
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden,
                            "Nemaš pristup izvještajima iz ovog odjela.");

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = model.ReportID });
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentsSelectList(db, model.DepartmentID);
                PopulateEmployeesSelectList(db, model.CreatedBy);
                ViewBag.StatusList = new SelectList(StatusValues, model.Status);
                ViewBag.ReportTypeList = new SelectList(ReportTypeValues, model.ReportType);
            }
            return View(model);
        }

        private void PopulateDepartmentsSelectList(StateStatisticsDBEntities db, int? selectedId)
        {
            var departments = db.Departments.OrderBy(x => x.DepartmentName).ToList();
            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "DepartmentName", selectedId);
        }

        private void PopulateEmployeesSelectList(StateStatisticsDBEntities db, int? selectedId)
        {
            var employees = db.Employees
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .Select(x => new { x.EmployeeID, FullName = x.LastName + " " + x.FirstName })
                .ToList();
            ViewBag.CreatedBy = new SelectList(employees, "EmployeeID", "FullName", selectedId);
        }
    }
}
