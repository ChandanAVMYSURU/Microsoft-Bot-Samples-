using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using demo.Models;

namespace demo.Controllers
{
    public class empsController : ApiController
    {
        private SwaggerDemoContext db = new SwaggerDemoContext();

        // GET: api/emps
        public IQueryable<emp> Getemps()
        {
            return db.emps;
        }

        // GET: api/emps/5
        [ResponseType(typeof(emp))]
        public IHttpActionResult Getemp(int id)
        {
            emp emp = db.emps.Find(id);
            if (emp == null)
            {
                return NotFound();
            }

            return Ok(emp);
        }

        // PUT: api/emps/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putemp(int id, emp emp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emp.EmployeeID)
            {
                return BadRequest();
            }

            db.Entry(emp).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!empExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/emps
        [ResponseType(typeof(emp))]
        public IHttpActionResult Postemp(emp emp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.emps.Add(emp);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = emp.EmployeeID }, emp);
        }

        // DELETE: api/emps/5
        [ResponseType(typeof(emp))]
        public IHttpActionResult Deleteemp(int id)
        {
            emp emp = db.emps.Find(id);
            if (emp == null)
            {
                return NotFound();
            }

            db.emps.Remove(emp);
            db.SaveChanges();

            return Ok(emp);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool empExists(int id)
        {
            return db.emps.Count(e => e.EmployeeID == id) > 0;
        }
    }
}