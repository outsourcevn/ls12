using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTMDT.Models;

namespace WebTMDT.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LocalsController : Controller
    {
        private langson12Entities db = new langson12Entities();

        // GET: Locals
        public async Task<ActionResult> Index()
        {
            var locals = db.Locals.Include(l => l.Local2);
            return View(await locals.ToListAsync());
        }

        // GET: Locals/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Local local = await db.Locals.FindAsync(id);
            if (local == null)
            {
                return HttpNotFound();
            }
            return View(local);
        }

        // GET: Locals/Create
        public ActionResult Create()
        {
            ViewBag.F3 = new SelectList(db.Locals, "F1", "F2");
            return View();
        }

        // POST: Locals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "F1,F2,F3")] Local local)
        {
            if (ModelState.IsValid)
            {
                db.Locals.Add(local);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.F3 = new SelectList(db.Locals, "F1", "F2", local.F3);
            return View(local);
        }

        // GET: Locals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Local local = await db.Locals.FindAsync(id);
            if (local == null)
            {
                return HttpNotFound();
            }
            ViewBag.F3 = new SelectList(db.Locals, "F1", "F2", local.F3);
            return View(local);
        }

        // POST: Locals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "F1,F2,F3")] Local local)
        {
            if (ModelState.IsValid)
            {
                db.Entry(local).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.F3 = new SelectList(db.Locals, "F1", "F2", local.F3);
            return View(local);
        }

        // GET: Locals/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Local local = await db.Locals.FindAsync(id);
            if (local == null)
            {
                return HttpNotFound();
            }
            return View(local);
        }

        // POST: Locals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Local local = await db.Locals.FindAsync(id);
            db.Locals.Remove(local);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
