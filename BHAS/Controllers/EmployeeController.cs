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
            return View();
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
            return View();
        }


        [HttpPost]
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

                return RedirectToAction("Details", "Department", new { id=model.DepartmentID }); // vrati se na listu zaposlenih
            }
            // nije validan => vrati se na formu 
            return View(model);
        }
    }
}