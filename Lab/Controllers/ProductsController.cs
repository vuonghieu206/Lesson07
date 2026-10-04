using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAppMVC.Models;

namespace MyAppMVC.Controllers;

public class ProductsController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private static readonly List<Category> Categories = new()
    {
        new Category { Id = 1, Name = "Danh mục 1" },
        new Category { Id = 2, Name = "Danh mục 2" },
        new Category { Id = 3, Name = "Danh mục 3" }
    };
    private static readonly List<Product> Products = new();
    private static int _nextId = 1;

    public ProductsController(IWebHostEnvironment environment) => _environment = environment;

    public IActionResult Index()
    {
        foreach (var product in Products)
            product.Category = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
        return View(Products.OrderBy(p => p.Id).ToList());
    }

    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();
        product.Category = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
        return View(product);
    }

    public IActionResult Create()
    {
        LoadCategories();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        // Lưu ảnh ngay khi nhận được để không mất ảnh nếu các trường khác chưa hợp lệ.
        ValidateImage(imageFile, required: string.IsNullOrWhiteSpace(product.Image));
        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (allowedExtensions.Contains(extension))
            {
                product.Image = await SaveImage(imageFile);
                ModelState.Remove(nameof(product.Image));
            }
        }
        else if (string.IsNullOrWhiteSpace(product.Image) ||
                 !System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "products", product.Image)))
        {
            ModelState.AddModelError(nameof(product.Image), "Vui lòng chọn ảnh sản phẩm.");
        }

        if (product.CategoryId == null || !Categories.Any(c => c.Id == product.CategoryId))
            ModelState.AddModelError(nameof(product.CategoryId), "Vui lòng chọn danh mục hợp lệ.");
        if (!ModelState.IsValid)
        {
            LoadCategories(product.CategoryId);
            return View(product);
        }

        product.Id = _nextId++;
        product.Category = Categories.First(c => c.Id == product.CategoryId);
        Products.Add(product);
        TempData["Message"] = "Thêm sản phẩm thành công.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();
        LoadCategories(product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
    {
        if (id != product.Id) return NotFound();
        var existing = Products.FirstOrDefault(p => p.Id == id);
        if (existing == null) return NotFound();

        product.Image = existing.Image;
        if (imageFile != null && imageFile.Length > 0)
            product.Image = imageFile.FileName;
        ValidateImage(imageFile, required: false);
        if (product.CategoryId == null || !Categories.Any(c => c.Id == product.CategoryId))
            ModelState.AddModelError(nameof(product.CategoryId), "Vui lòng chọn danh mục hợp lệ.");
        if (!ModelState.IsValid)
        {
            LoadCategories(product.CategoryId);
            return View(product);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            product.Image = await SaveImage(imageFile);
            DeleteImage(existing.Image);
        }
        existing.Name = product.Name;
        existing.Image = product.Image;
        existing.Price = product.Price;
        existing.SalePrice = product.SalePrice;
        existing.Description = product.Description;
        existing.CategoryId = product.CategoryId;
        existing.Category = Categories.First(c => c.Id == product.CategoryId);
        TempData["Message"] = "Cập nhật sản phẩm thành công.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int? id)
    {
        if (id == null) return NotFound();
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();
        product.Category = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            Products.Remove(product);
            DeleteImage(product.Image);
        }
        TempData["Message"] = "Xóa sản phẩm thành công.";
        return RedirectToAction(nameof(Index));
    }

    private void LoadCategories(int? selectedId = null) =>
        ViewBag.CategoryId = new SelectList(Categories.OrderBy(c => c.Name), "Id", "Name", selectedId);

    private void ValidateImage(IFormFile? file, bool required)
    {
        if (file == null || file.Length == 0)
        {
            if (required) ModelState.AddModelError("Image", "Vui lòng chọn ảnh sản phẩm.");
            return;
        }
        var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!extensions.Contains(extension))
            ModelState.AddModelError("Image", "Ảnh phải có định dạng JPG, JPEG, PNG, GIF hoặc WEBP.");
    }

    private async Task<string> SaveImage(IFormFile file)
    {
        var folder = Path.Combine(_environment.WebRootPath, "products");
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        await using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }

    private void DeleteImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;
        var path = Path.Combine(_environment.WebRootPath, "products", fileName);
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
    }
}
