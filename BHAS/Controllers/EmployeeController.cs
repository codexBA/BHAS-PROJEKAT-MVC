using BHAS.DbFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    public class EmployeeController : Controller
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

        public ActionResult Create()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var departments = db.Departments
                    .OrderBy(x => x.DepartmentName)
                    .ToList();

                ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "DepartmentName", null);
            }
            //
            return View();
        }


        /// <summary>
        /// POST - brisanje zaposlenika 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee model)
        {
            //if (model.DepartmentID < 1)
            //   ModelState.AddModelError("DepartmentID", "Odaberi odjel");

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
            // nije validan => vrati se na formu 
            return View(model);
        }
    }
}