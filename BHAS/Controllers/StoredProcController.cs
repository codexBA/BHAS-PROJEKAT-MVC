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


        public ActionResult RegionalStats(int? year)
        {
            using (var db = new StateStatisticsDBEntities())
            {

                var years = db.EconomicDatas
                              .Select(x => x.Year)
                              .Distinct()
                              .OrderByDescending(y => y)
                              .ToList();
                ViewBag.Years = new SelectList(years);

                if (!year.HasValue)
                    return View((object)null);


                ViewBag.SelectedYear = year.Value;
                var p = new SqlParameter("@Year", year.Value);
                var list = db.Database
                             .SqlQuery<RegionalStatsViewModel>(
                                 "EXEC Stats.sp_GetRegionalStatsByYear @Year",
                                 p)
                             .ToList();
                return View(list);
            }
        }


        public ActionResult SalaryRaise()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db);
                return View();

            }
        }

        public ActionResult ByDepartment(int? departmentId)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateDepartmentSelect(db, departmentId);

                if (!departmentId.HasValue)
                    return View((object)null);

                var p = new SqlParameter("@DepartmentID", departmentId.Value);
                var list = db.Database
                             .SqlQuery<Employee>(
                                 "EXEC Stats.sp_GetEmployeesByDepartment @DepartmentID",
                                 p)
                             .ToList();

                ViewBag.FilterDepartment = db.Departments.Find(departmentId.Value)?.DepartmentName;
                return View(list);
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
            using (var db = new StateStatisticsDBEntities())
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



        public ActionResult UpdateProjectStatus()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateProjectsSelectList(db, null);
                PopulateStatusSelectList(null);
                return View();
            }
        }

        // POST: StoredProc/UpdateProjectStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProjectStatus(int projectId, string newStatus)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var p1 = new SqlParameter("@ProjectID", projectId);
                var p2 = new SqlParameter("@NewStatus", newStatus);

                int rows = db.Database.ExecuteSqlCommand(
                    "EXEC Stats.sp_UpdateProjectStatus @ProjectID, @NewStatus",
                    p1, p2);

                ViewBag.RowsAffected = rows;
                PopulateProjectsSelectList(db, projectId);
                PopulateStatusSelectList(newStatus);
                return View();
            }
        }


        #region -- privatne metode --

        private void PopulateProjectsSelectList(StateStatisticsDBEntities db, int? selectedId)
        {
            var projects = db.Projects.OrderBy(x => x.ProjectName).ToList();
            ViewBag.ProjectID = new SelectList(projects, "ProjectID", "ProjectName", selectedId);
        }

        private void PopulateStatusSelectList(string selectedStatus)
        {
            var statuses = new[]
            {
                new { Value = "Planned",    Text = "Planiran" },
                new { Value = "InProgress", Text = "U toku" },
                new { Value = "Completed",  Text = "Završen" },
                new { Value = "Cancelled",  Text = "Otkazan" }
            };
            ViewBag.StatusList = new SelectList(statuses, "Value", "Text", selectedStatus);
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

        #endregion -- privatne metode --
    }
}