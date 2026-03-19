using BHAS.DbFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHAS.Models
{
    public class DepartmentSummaryViewModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public string DepartmentCode { get; set; }
        public decimal Budget { get; set; }

        public string ManagerFullName { get; set; }

        public int EmployeeCount { get; set; }

        public decimal AverageSalary { get; set; }

        public decimal TotalSalaryBill { get; set; }

        public int TotalProjectCount { get; set; }

        public int ActiveProjectCount { get; set; }

        public decimal TotalProjectBudget { get; set; }
    }
}