﻿using ShoppingCard.Models.Data;
using ShoppingCard.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCard.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declare list od PageVM
            List<PageVM> pagesList;
            
            using (Db db = new Db())
            {
                // Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            // Retrn view with list

            return View(pagesList);
        }
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Check model state
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                // Declare slug
                string slug;

                // Init pageDTO
                PageDTO dto = new PageDTO();

                // DTO title
                dto.Title = model.Title;

                // Check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Make sure title and sluge are unique
                if(db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The title or slug already exists.");
                    return View(model);
                }

                // DTO the reset
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            } 

            // Set TempData message
            TempData["SM"] = "You have add a new page!";

            // Redirect
            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declare pageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exist
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Init pageVM
                model = new PageVM(dto);

            }


            // Return view with model
            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Chech model state
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Get page id
                int id = model.Id;

                // Init slug
                string slug = "home";

                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // DTO the title
                dto.Title = model.Title;

                // Check for slug and set it if need be
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // Make sure title and slug are unique
                if(db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || 
                   db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The title or slug already exists.");
                    return View(model);
                }

                // DTO the reset
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save the DTO
                db.SaveChanges();

            }

            // Set TemData message
            TempData["SM"] = "You have the edited the page!";

            // Redirect
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Declare pageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exist
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                // Init pageVM
                model = new PageVM(dto);
            }

            // Return view with model
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Remove the page
                db.Pages.Remove(dto);

                // Save
                db.SaveChanges();
            }

            // Redirec

            return RedirectToAction("Index");
        }

        

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declare model
            SidebarVM model;

            using(Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                // Init model
                model = new SidebarVM(dto);
            }

            // Return view with model
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                // DTO the body
                dto.Body = model.Body;

                // Save
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You have edited the sidebar!";

            // Redirect
            return RedirectToAction("EditSidebar");
        }

    }
}