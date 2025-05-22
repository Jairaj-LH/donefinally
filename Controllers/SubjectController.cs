using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace charac.Controllers
{
    //inherit from controller baseclass
    public class SubjectController:Controller
    {
        //first inject the dbcontext
        //Your controller needs access to the database,
        //so you’ll inject ApplicationDbContext.
        private readonly ApplicationDbContext _db;
        public SubjectController(ApplicationDbContext db)
        {
            _db = db;
        }
        //show all subjects in indexmethod
        public async Task<IActionResult> Index()
        {
            var subjects = await _db.Subjects.ToListAsync();
            return View(subjects);     
        }

        //show form to create a subject
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();  //GET actions typically show forms or pages
                            //without any data when you just need a form..
        }

        //Handle the form submission post
        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            _db.Add(subject);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }  
        //view details of a subject 
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();
            return View(subject);
        }

        //Edit (get + post)
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();
            return View(subject);

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id,Subject subject)
        {
            if (id != subject.SubId)
                return NotFound();

            _db.Subjects.Update(subject);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Delete(Get+Post)
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (id == null)
                return NotFound();
            return View(subject);
        }
        [HttpPost]

        public async Task<IActionResult> DeletePost(int SubId)
        {
            var subject = await _db.Subjects.FindAsync(SubId);
            if (subject==null)  //remember you should always include subid as hidded input in form,otherwise efcore automatically allocates
                                      //the default value for subid(not actual subid is assigned) and hence the condition becomes true and your record doesn't deletes
                return NotFound();

            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
