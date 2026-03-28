using BHAS.DbFirst;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        // Ne koristi se trenutno
        public ActionResult Index()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                return View(db.Employees.ToList());
            }
        }

        /// <summary>
        /// Prikaz detalja zaposlenika
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            using (var db = new BHAS.DbFirst.StateStatisticsDBEntities())
            {
                // postoji razlika izmedju First i FirstOrDefault
                var zaposleni = db.Employees
                                        .FirstOrDefault(e => e.EmployeeID == id);
                return View(zaposleni);
            }
        }



        /// <summary>
        /// POST - brisanje zaposlenika 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Delete(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var employeeToDelete = db.Employees.Find(id);
                // var employeeToDelete = db.Employees.FirstOrDefault(emp => emp.EmployeeID == id);
                if (employeeToDelete == null)
                    return HttpNotFound();
                // uzimamo ID odjela kako bi definisali putanju za vracanje: prikaz detalja odjela
                var departmentId = employeeToDelete.DepartmentID;
                //
                db.Employees.Remove(employeeToDelete);
                db.SaveChanges(); // ovo je obavezno i okida snimanje naznacenih izmjena

                // vracemo se na listu detalja odjela koji prikazuje i listu zaposlenih u njemu
                return RedirectToAction("Details", "Department", new { id = departmentId });
            }
        }


        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var employee = db.Employees.Find(id);
                if (employee == null)
                    return HttpNotFound();

                if (!CanEditDepartment(employee.DepartmentID))
                    return DepartmentAccessDenied();

                PopulateDepartmentSelect(db, employee.DepartmentID);
                return View(employee);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    var existing = db.Employees.Find(model.EmployeeID);
                    if (existing != null && !CanEditDepartment(existing.DepartmentID))
                        return DepartmentAccessDenied();

                    model.ModifiedDate = DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index"); // vraca na listu svih zaposlenh
                //return RedirectToAction("Details", "Department", new { id = model.DepartmentID }); // vrati se na listu zaposlenih
            }
            //
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db, model.DepartmentID);
            }
            // nije validan => vrati se na formu 
            return View(model);
        }

        /// <summary>
        /// Ova metoda dobavlja listu odjela i kreira 'Select' koji ce se koristiti u Views (u Create i Edit)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectedDepartmentId"></param>
        private void PopulateDepartmentSelect(StateStatisticsDBEntities db, int? selectedDepartmentId = null)
        {
            var departments = db.Departments
                   .OrderBy(x => x.DepartmentName)
                   .ToList();

            ViewBag.DepartmentID = new SelectList(departments,
                                                    "DepartmentID",
                                                    "DepartmentName",
                                                    selectedDepartmentId);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db);
            }
            //
            return View();
        }




        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                // podaci su ispravni => snimi u bazu
                using (var db = new StateStatisticsDBEntities())
                {
                    // definisemo vrijednosti koje nisu odredjene kroz formu
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    // dodavanje
                    try
                    {
                        db.Employees.Add(model);
                        // snimanje
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // treba zapisati log
                        return View(model);
                    }
                }
                return RedirectToAction("Details", "Department", new { id = model.DepartmentID }); // vrati se na listu zaposlenih
            }
            //
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db);
            }
            // nije validan => vrati se na formu 
            return View(model);
        }
    }
}