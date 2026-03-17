using BHAS.DbFirst;
using System;
using System.Collections.Generic;
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

        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var dpt = db.Departments
                    .FirstOrDefault(x => x.DepartmentID == id);

                return View(dpt);
            }
        }
    }
}