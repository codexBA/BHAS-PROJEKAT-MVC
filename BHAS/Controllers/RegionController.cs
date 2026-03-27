using BHAS.Controllers;
using BHAS.DbFirst;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AS.MVCDemo.Controllers
{
    [Authorize]
    public class RegionController : BaseController
    {
        // GET: Region
        public ActionResult Index()
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var regions = db.Regions
                    .OrderBy(x => x.RegionName)
                    .ToList();
                return View(regions);
            }
        }

        // GET: Region/Details/5
        public ActionResult Details(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var region = db.Regions
                    .Include(x => x.EconomicDatas)
                    .Include(x => x.ReportDatas)
                    .FirstOrDefault(x => x.RegionID == id);

                if (region == null)
                    return HttpNotFound();

                return View(region);
            }
        }

        // GET: Region/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Region/Create
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RegionName,RegionCode,Population,Area")] Region model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    db.Regions.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Region/Edit/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Edit(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var region = db.Regions.Find(id);
                if (region == null)
                    return HttpNotFound();
                return View(region);
            }
        }

        // POST: Region/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegionID,RegionName,RegionCode,Population,Area")] Region model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new StateStatisticsDBEntities())
                {
                    model.ModifiedDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Region/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult Delete(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var region = db.Regions.Find(id);
                if (region == null)
                    return HttpNotFound();
                return View(region);
            }
        }

        // POST: Region/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                try
                {
                    var region = db.Regions.Find(id);
                    if (region == null)
                        return HttpNotFound();

                    db.Regions.Remove(region);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["Error"] = "Nije moguće obrisati regiju jer ima vezanih podataka (ekonomski podaci ili podaci izvještaja).";
                    return RedirectToAction("Details", new { id });
                }
            }
        }

        // GET: Region/EconomicDataPartial?regionId=5
        [HttpGet]
        public ActionResult EconomicDataPartial(int regionId)
        {
            using (var db = new StateStatisticsDBEntities())
            {
                var data = db.EconomicDatas
                    .Where(x => x.RegionID == regionId)
                    .OrderByDescending(x => x.Year)
                    .ToList();
                return PartialView("_EconomicDataTable", data);
            }
        }
    }
}
