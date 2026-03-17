using BHAS.DbFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    public class DepartmentController : Controller
    {
        /// <summary>
        /// Vraca listu svih odjela iz baze i proslijedjuje u view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (BHAS.DbFirst.StateStatisticsDBEntities db = new StateStatisticsDBEntities())
            {
                List<BHAS.DbFirst.Department> dpList
                    = db.Departments.ToList();
                //
                return View(dpList);
            }
        }


        /// <summary>
        /// Prikaz detalja odjela - ukljucuje i poveznicu sa employee/manager
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var dpt = db.Departments
                    .Include(x => x.Employee)
                    .Include(x => x.Employees)
                    .FirstOrDefault(x => x.DepartmentID == id);
                //
                return View(dpt);
            }
        }
    }
}