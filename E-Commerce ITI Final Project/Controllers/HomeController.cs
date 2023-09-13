using E_Commerce_ITI_Final_Project.Data;
using E_Commerce_ITI_Final_Project.Entities;
using E_Commerce_ITI_Final_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace E_Commerce_ITI_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreContext storeContext;

        public HomeController(StoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        //[Authorize(Roles = "User")]
        public IActionResult Index(string? ProductName = null)
        {
            List<Product> allProducts;
            if (string.IsNullOrEmpty(ProductName))
                allProducts = storeContext.Products.Include("Seller").Include("Category").ToList();
            else
            {
                allProducts = storeContext.Products.Include("Seller").Include("Category").Where(p => p.Name.Contains(ProductName)).ToList();
                ViewBag.Search = ProductName;
            }
            var user = storeContext.Accounts.FirstOrDefault(u => u.Email == ClaimTypes.Email);

            if (user != null)
            {
                ViewBag.User = user;
            }

            return View(allProducts);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}