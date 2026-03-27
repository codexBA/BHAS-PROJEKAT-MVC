using BHAS.Controllers;
using BHAS.DbFirst;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AS.MVCDemo.Controllers
{
    [Authorize]
    public class EconomicDataController : BaseController
    {
        // GET: EconomicData
        public ActionResult Index(int? regionId)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var query = db.EconomicDatas.Include(x => x.Region).AsQueryable();

                if (regionId.HasValue)
                {
                    query = query.Where(x => x.RegionID == regionId.Value);
                    ViewBag.FilterRegion = db.Regions.Find(regionId.Value)?.RegionName;
                }

                var data = query
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Region.RegionName)
                    .ToList();

                PopulateRegionsSelectList(db, regionId);
                return View(data);
            }
        }

        // GET: EconomicData/Details/5
        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var item = db.EconomicDatas
                    .Include(x => x.Region)
                    .FirstOrDefault(x => x.DataID == id);

                if (item == null)
                    return HttpNotFound();

                return View(item);
            }
        }

        // GET: EconomicData/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create(int? regionId)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                PopulateRegionsSelectList(db, regionId);
                var model = new EconomicData
                {
                    Year = DateTime.Now.Year,
                    RegionID = regionId ?? 0
                };
                return View(model);
            }
        }

        // POST: EconomicData/Create
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RegionID,Year,GDP,UnemploymentRate,AverageSalary,InflationRate,RecordedDate")] EconomicData model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.RecordedDate = DateTime.Now;
                    db.EconomicDatas.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Region", new { id = model.RegionID });
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateRegionsSelectList(db, model.RegionID);
            }
            return View(model);
        }

        // GET: EconomicData/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var item = db.EconomicDatas
                    .Include(x => x.Region)
                    .FirstOrDefault(x => x.DataID == id);

                if (item == null)
                    return HttpNotFound();

                PopulateRegionsSelectList(db, item.RegionID);
                return View(item);
            }
        }

        // POST: EconomicData/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DataID,RegionID,Year,GDP,UnemploymentRate,AverageSalary,InflationRate,RecordedDate")] EconomicData model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Region", new { id = model.RegionID });
                }
            }

            using (var db = new StateStatisticsDBEntities())
            {
                PopulateRegionsSelectList(db, model.RegionID);
            }
            return View(model);
        }

        // GET: EconomicData/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Delete(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var item = db.EconomicDatas
                    .Include(x => x.Region)
                    .FirstOrDefault(x => x.DataID == id);

                if (item == null)
                    return HttpNotFound();

                return View(item);
            }
        }

        // POST: EconomicData/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var item = db.EconomicDatas.Find(id);
                if (item == null)
                    return HttpNotFound();

                int regionId = item.RegionID;
                db.EconomicDatas.Remove(item);
                db.SaveChanges();
                return RedirectToAction("Details", "Region", new { id = regionId });
            }
        }

        private void PopulateRegionsSelectList(StateStatisticsDBEntities db, int? selectedId)
        {
            var regions = db.Regions.OrderBy(x => x.RegionName).ToList();
            ViewBag.RegionID = new SelectList(regions, "RegionID", "RegionName", selectedId);
        }
    }
}
