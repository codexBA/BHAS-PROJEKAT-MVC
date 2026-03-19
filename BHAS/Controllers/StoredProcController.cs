using BHAS.DbFirst;
using BHAS.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace BHAS.Controllers
{
    public class StoredProcController : Controller
    {
        // GET: StoredProc
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Prikazuje listu svih zaposlenika. 
        /// Poziva SP: [Stats].[sp_GetAllEmployees]
        /// </summary>
        /// <returns></returns>
        public ActionResult AllEmployees()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var query = db.Database.SqlQuery<Employee>("Exec [Stats].[sp_GetAllEmployees]");
                var employeess = query.ToList();
                //
                return View(employeess);
            }
        }

        public ActionResult AllEmployeesQuery()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var query = db.Database.SqlQuery<Employee>("SELECT * FROM Stats.Employees");
                var employeess = query.ToList();
                //
                return View("AllEmployees", employeess);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public ActionResult SearchEmployees(int? deptId, decimal? min, decimal? max)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db, deptId);

                var p1 = new SqlParameter("@DepartmentID", (object)deptId ?? System.DBNull.Value);
                var p2 = new SqlParameter("@MinSalary", (object)min ?? DBNull.Value);
                var p3 = new SqlParameter("@MaxSalary", (object)max ?? DBNull.Value);

                var query = db.Database
                    .SqlQuery<Employee>(
                        "Exec [Stats].[sp_SearchEmployees] @DepartmentID, @MinSalary, @MaxSalary",
                         p1, p2, p3
                        );

                var employeess = query.ToList();

                return View(employeess);
            }
        }

        // [Stats].[sp_GetDepartmentSummary]

        public ActionResult DepartmentSummary()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var query = db.Database
                    .SqlQuery<DepartmentSummaryViewModel>("[Stats].[sp_GetDepartmentSummary]");

                var summaryStat = query.ToList();

                return View(summaryStat);
            }
        }


        public ActionResult SalaryRaise()
        {
            using(var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db);
                return View();

            }
        }

        /// <summary>
        /// POST - update podataka u bazi
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SalaryRaise(int departmentId, decimal percentage)
        {
            using(var db = new StateStatisticsDBEntities())
            {
                var p1 = new SqlParameter("@DepartmentID", departmentId);
                var p2 = new SqlParameter("@PercentageIncrease", percentage);

                var count = db.Database
                    .ExecuteSqlCommand("[Stats].[sp_GiveSalaryRaise] @DepartmentID, @PercentageIncrease", 
                    p1, p2);
                //
                ViewBag.RowsAffected = count;
                ViewBag.PercentageIncrease = percentage;
                //
                PopulateDepartmentSelect(db, departmentId);
                //
                return View();
            }
        }



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
    }
}