using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace UniversityCafeteria.Controllers
{
    public class SupplyRequestController : Controller
    {
        private static List<string> confirmedRequests = new();

        public IActionResult Index()
        {
            ViewBag.PreferredPriority =
                Request.Cookies["PreferredPriority"] ?? "Normal";

            return View();
        }

        [HttpPost]
        public IActionResult AddItem(
            string ProductName,
            string Category,
            int Quantity,
            decimal UnitPrice,
            string Priority)
        {
            if (string.IsNullOrWhiteSpace(ProductName) ||
                Quantity <= 0 || UnitPrice <= 0)
            {
                TempData["Error"] = "Invalid product information";
                return RedirectToAction("Index");
            }

            if (Priority != "Normal" &&
                Priority != "Urgent" &&
                Priority != "Emergency")
            {
                TempData["Error"] = "Invalid priority";
                return RedirectToAction("Index");
            }

            var products = GetProducts();

            products.Add(new string[]
            {
                ProductName,
                Category ?? "",
                Quantity.ToString(),
                UnitPrice.ToString(CultureInfo.InvariantCulture),
                Priority
            });
            //list -> json
            HttpContext.Session.SetString(
                "Products", JsonSerializer.Serialize(products));

            return RedirectToAction("Review");
        }

        public IActionResult Review()
        {
            var products = GetProducts();

            decimal subtotal = 0;
            string priority = "Normal";

            foreach (var product in products)
            {
                int quantity = int.Parse(product[2]);
                decimal price = decimal.Parse(product[3]);

                subtotal += quantity * price;
                priority = product[4];
            }

            decimal fee = 0;

            switch (priority)
            {
                case "Normal":
                    fee = 2;
                    break;

                case "Urgent":
                    fee = 5;
                    break;

                case "Emergency":
                    fee = 10;
                    break;
            }

            decimal discount = 0;

            if (subtotal > 200)
                discount = subtotal * 0.15m;
            else if (subtotal > 100)
                discount = subtotal * 0.10m;

            decimal finalTotal = subtotal + fee - discount;

            ViewBag.Products = products;
            ViewBag.Subtotal = subtotal;
            ViewBag.Fee = fee;
            ViewBag.Discount = discount;
            ViewBag.FinalTotal = finalTotal;
            ViewBag.Priority = priority;

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmRequest()
        {
            var products = GetProducts();

            if (products.Count == 0)
            {
                TempData["Error"] = "Cannot confirm an empty request";
                return RedirectToAction("Review");
            }

            // Calculate totals using Review logic
            decimal subtotal = 0;
            string priority = "Normal";

            foreach (var product in products)
            {
                subtotal += int.Parse(product[2]) *
                    decimal.Parse(product[3]);

                priority = product[4];
            }

            decimal fee = priority switch
            {
                "Urgent" => 5,
                "Emergency" => 10,
                _ => 2
            };

            decimal discount = subtotal > 200
                ? subtotal * 0.15m
                : subtotal > 100
                    ? subtotal * 0.10m
                    : 0;

            decimal total = subtotal + fee - discount;

            lock (confirmedRequests)
            {
                int id = confirmedRequests.Count + 1;

                confirmedRequests.Add(
                    $"{id}|{products.Count}|{subtotal}|" +
                    $"{fee}|{discount}|{total}|Confirmed");
            }

            HttpContext.Session.Remove("Products");

            return RedirectToAction("MyRequests");
        }

        [HttpPost]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove("Products");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SavePreference(string Priority)
        {
            if (Priority != "Normal" &&
                Priority != "Urgent" &&
                Priority != "Emergency")
            {
                return BadRequest();
            }

            Response.Cookies.Append(
                "PreferredPriority",
                Priority,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });

            return RedirectToAction("Index");
        }

        public IActionResult MyRequests()
        {
            lock (confirmedRequests)
            {
                ViewBag.Requests = confirmedRequests.ToList();
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        private List<string[]> GetProducts()
        {
            string? json =
                HttpContext.Session.GetString("Products");

            if (string.IsNullOrEmpty(json))
                return new List<string[]>();


            //Deserialize بتحوّل الـ JSON String اللي جبناه من Session إلى List<string[]>.
            return JsonSerializer.Deserialize<List<string[]>>(json)
                   ?? new List<string[]>();
        }
    }
}