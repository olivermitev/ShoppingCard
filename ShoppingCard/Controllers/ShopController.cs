﻿using ShoppingCard.Models.Data;
using ShoppingCard.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCard.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // Declare list od CategoryVM
            List<CategoryVM> categoryVMLIst;

            // Init the list
            using (Db db = new Db())
            {
                categoryVMLIst = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            // Return partial with list
            return PartialView(categoryVMLIst);
        }

        // GET: /shop/category/name
        public ActionResult Category(string name)
        {
            // Declare list od ProductVM
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // Get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                // Init the list
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                // Get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }


            // return view with list

            return View(productVMList);
        }

        // GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Declare the VM and DTO
            ProductVM model;
            ProductDTO dto;

            // Init product id
            int id = 0;

            using (Db db = new Db())
            {
                // Check if product exists
                if(! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Init productDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Get inserted id
                id = dto.Id;

                // Init model
                model = new ProductVM(dto);
            }

            // Get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));

            // Return view with model

            return View("ProductDetails", model);
        }

    }
}