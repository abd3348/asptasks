using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task6.Data;
using task6.Models;
using task6.Models.ViewModels;

namespace task6.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplyRequestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.SupplyRequests
                .Include(r => r.User)
                .Include(r => r.RequestItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (!IsAdministrator())
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(r => r.UserId == userId);
            }

            return View(await query.OrderByDescending(r => r.RequestDate).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new SupplyRequestFormViewModel
            {
                Products = await BuildProductListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplyRequestFormViewModel model)
        {
            var selected = model.Products.Where(p => p.Quantity > 0).ToList();
            if (selected.Count == 0)
                ModelState.AddModelError(string.Empty, "Select at least one product.");

            var products = await _context.Products
                .Where(p => selected.Select(s => s.ProductId).Contains(p.Id))
                .ToListAsync();

            ValidateQuantities(selected, products);
            if (!ModelState.IsValid)
            {
                model.Products = await BuildProductListAsync(model.Products);
                return View(model);
            }

            var request = new SupplyRequest
            {
                RequestDate = DateTime.UtcNow,
                Status = SupplyRequest.Pending,
                UserId = _userManager.GetUserId(User)!,
                RequestItems = selected.Select(item =>
                {
                    var product = products.Single(p => p.Id == item.ProductId);
                    return new RequestItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };
                }).ToList()
            };

            request.TotalPrice = request.RequestItems.Sum(i => i.Price * i.Quantity);
            _context.SupplyRequests.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await GetRequestAsync(id);
            if (request == null)
                return NotFound();
            if (!CanAccess(request))
                return Forbid();

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await GetRequestAsync(id);
            if (request == null)
                return NotFound();
            if (!CanEdit(request))
                return Forbid();

            var quantities = request.RequestItems.ToDictionary(i => i.ProductId, i => i.Quantity);
            return View(new SupplyRequestFormViewModel
            {
                Id = request.Id,
                Status = request.Status,
                Products = await BuildProductListAsync(quantities.Select(q =>
                    new RequestProductViewModel { ProductId = q.Key, Quantity = q.Value }).ToList())
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplyRequestFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var request = await GetRequestAsync(id);
            if (request == null)
                return NotFound();
            if (!CanEdit(request))
                return Forbid();

            var selected = model.Products.Where(p => p.Quantity > 0).ToList();
            if (selected.Count == 0)
                ModelState.AddModelError(string.Empty, "Select at least one product.");

            var products = await _context.Products
                .Where(p => selected.Select(s => s.ProductId).Contains(p.Id))
                .ToListAsync();
            ValidateQuantities(selected, products);

            if (IsAdministrator())
            {
                var validStatuses = new[]
                {
                    SupplyRequest.Pending,
                    SupplyRequest.Approved,
                    SupplyRequest.Rejected
                };
                if (!validStatuses.Contains(model.Status))
                    ModelState.AddModelError(nameof(model.Status), "Invalid status.");
            }
            else
            {
                model.Status = SupplyRequest.Pending;
            }

            if (!ModelState.IsValid)
            {
                model.Products = await BuildProductListAsync(model.Products);
                return View(model);
            }

            _context.RequestItems.RemoveRange(request.RequestItems);
            request.RequestItems = selected.Select(item =>
            {
                var product = products.Single(p => p.Id == item.ProductId);
                return new RequestItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
            }).ToList();
            request.TotalPrice = request.RequestItems.Sum(i => i.Price * i.Quantity);
            request.Status = model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await GetRequestAsync(id);
            if (request == null)
                return NotFound();
            if (!CanEdit(request))
                return Forbid();

            _context.SupplyRequests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IsAdministrator() =>
            User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

        private bool CanAccess(SupplyRequest request) =>
            IsAdministrator() || request.UserId == _userManager.GetUserId(User);

        private bool CanEdit(SupplyRequest request) =>
            IsAdministrator() ||
            (request.UserId == _userManager.GetUserId(User) && request.Status == SupplyRequest.Pending);

        private async Task<SupplyRequest?> GetRequestAsync(int id) =>
            await _context.SupplyRequests
                .Include(r => r.User)
                .Include(r => r.RequestItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

        private async Task<List<RequestProductViewModel>> BuildProductListAsync(
            IEnumerable<RequestProductViewModel>? submitted = null)
        {
            var quantities = submitted?.ToDictionary(i => i.ProductId, i => i.Quantity)
                ?? new Dictionary<int, int>();

            return await _context.Products.OrderBy(p => p.Name).Select(p =>
                new RequestProductViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    AvailableQuantity = p.Quantity,
                    Quantity = quantities.ContainsKey(p.Id) ? quantities[p.Id] : 0
                }).ToListAsync();
        }

        private void ValidateQuantities(
            IEnumerable<RequestProductViewModel> selected,
            IReadOnlyCollection<Product> products)
        {
            foreach (var item in selected)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, "A selected product no longer exists.");
                else if (item.Quantity > product.Quantity)
                    ModelState.AddModelError(string.Empty,
                        $"Requested quantity for {product.Name} exceeds available quantity.");
            }
        }
    }
}
