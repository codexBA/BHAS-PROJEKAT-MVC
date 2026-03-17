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
                var zaposleni = db.Employees.FirstOrDefault(e => e.EmployeeID == id);
                return View(zaposleni);
            }
        }
    }
}