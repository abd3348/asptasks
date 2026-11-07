using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task4.Models;

namespace AcademyGraduationSupplyManagement.Controllers;

public class SupplyRequestsController : Controller
{
    private const string SessionKey = "SelectedOffers";

    private static readonly List<SelectedOfferViewModel> Offers = new()
    {
        new() { CategoryName = "Graduation Certificates", SupplierName = "Supplier A",
            OfferName = "Premium Certificate Package", Quantity = 35, UnitPrice = 2m },
        new() { CategoryName = "Graduation Certificates", SupplierName = "Supplier B",
            OfferName = "Standard Certificate Package", Quantity = 35, UnitPrice = 1.5m },
        new() { CategoryName = "Graduation Certificates", SupplierName = "Supplier C",
            OfferName = "Luxury Certificate Package", Quantity = 35, UnitPrice = 2.5m },

        new() { CategoryName = "Hospitality", SupplierName = "Supplier D",
            OfferName = "Coffee & Drinks Package", Quantity = 100, UnitPrice = 2m },
        new() { CategoryName = "Hospitality", SupplierName = "Supplier E",
            OfferName = "Sandwiches & Snacks Package", Quantity = 100, UnitPrice = 3m },
        new() { CategoryName = "Hospitality", SupplierName = "Supplier F",
            OfferName = "Full Hospitality Package", Quantity = 100, UnitPrice = 4m },

        new() { CategoryName = "Graduation Gifts", SupplierName = "Supplier G",
            OfferName = "Customized Mugs Package", Quantity = 35, UnitPrice = 2m },
        new() { CategoryName = "Graduation Gifts", SupplierName = "Supplier H",
            OfferName = "Graduation Bags Package", Quantity = 35, UnitPrice = 3m },
        new() { CategoryName = "Graduation Gifts", SupplierName = "Supplier I",
            OfferName = "Premium Gift Boxes Package", Quantity = 35, UnitPrice = 4m }
    };

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Offers = Offers;
        ViewBag.SelectedOffers = GetSelectedOffers();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectOffer(int offerIndex)
    {
        if (offerIndex < 0 || offerIndex >= Offers.Count)
            return RedirectToAction(nameof(Create));

        var offer = Offers[offerIndex];
        var selected = GetSelectedOffers();

        // عرض واحد فقط لكل فئة. الاختيار الجديد يستبدل السابق من الفئة نفسها.
        selected.RemoveAll(item => item.CategoryName == offer.CategoryName);
        selected.Add(offer);

        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(selected));

        return RedirectToAction(nameof(Create));
    }

    [HttpGet]
    public IActionResult Review()
    {
        var selected = GetSelectedOffers();

        if (selected.Count != 3)
        {
            TempData["Message"] = "Please select one offer from each category first.";
            return RedirectToAction(nameof(Create));
        }

        // الحسابات كلها داخل الـ Controller
        foreach (var offer in selected)
        {
            offer.TotalPrice = offer.Quantity * offer.UnitPrice;

            if (offer.TotalPrice > 500)
                offer.DiscountPercentage = 15;
            else if (offer.TotalPrice > 300)
                offer.DiscountPercentage = 10;
            else
                offer.DiscountPercentage = 0;

            offer.DiscountAmount =
                offer.TotalPrice * offer.DiscountPercentage / 100;

            offer.FinalTotal = offer.TotalPrice - offer.DiscountAmount;
        }

        ViewBag.GrandTotal = selected.Sum(offer => offer.TotalPrice);
        ViewBag.TotalDiscount = selected.Sum(offer => offer.DiscountAmount);
        ViewBag.FinalTotal = selected.Sum(offer => offer.FinalTotal);

        return View(selected);
    }

    private List<SelectedOfferViewModel> GetSelectedOffers()
    {
        var json = HttpContext.Session.GetString(SessionKey);

        return string.IsNullOrEmpty(json)
            ? new List<SelectedOfferViewModel>()
            : JsonSerializer.Deserialize<List<SelectedOfferViewModel>>(json)
              ?? new List<SelectedOfferViewModel>();
    }
}