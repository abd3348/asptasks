using Microsoft.AspNetCore.Mvc;

namespace UniversityCafeteria.Controllers
{
    public class SupplyController : Controller
    {
        public IActionResult Index()
        {
            string[] categories =
            {
                "Drinks",
                "Food",
                "Snacks",
                "Hot Drinks",
                "Cold Drinks"
            };

            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Products()
        {
            ViewData["Title"] = "Cafeteria Products";
            ViewData["Manager"] = "Ahmad Ali";
            ViewData["Count"] = 5;
            string[] products = { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
            ViewData["products"] = products;
            return View();
        }

        public IActionResult Suppliers()
        {
            ViewBag.Title = "Our Suppliers";
            ViewBag.Count = 3;

            string[] suppliers =
            {
                "Fresh Food Company",
                "Happy Drinks Company",
                "Daily Supplies Company"
            };

            ViewBag.Suppliers = suppliers;

            return View();
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitRequest()
        {
            string employeeName = Request.Form["EmployeeName"].ToString();
            string productName = Request.Form["ProductName"].ToString();
            string quantity = Request.Form["Quantity"].ToString();
            string requiredDate = Request.Form["RequiredDate"].ToString();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                ViewBag.Message = "Employee Name is required!";
                ViewBag.Success = false;

                return View();
            }

            TempData["Message"] = "Request submitted successfully!";

            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
