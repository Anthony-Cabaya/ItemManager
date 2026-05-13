using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class UnitCategoryController : BaseController
    {
        private readonly IUnitCategoryRepository _categoryRepo;

        public UnitCategoryController(
            IUnitCategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public class DeleteCategoriesRequest
        {
            public List<int> Ids { get; set; } = new();
        }

        private JsonResult JsonSuccess(
            string message,
            object? data = null)
        {
            return Json(new
            {
                success = true,
                message,
                data
            });
        }

        private JsonResult JsonFail(string message)
        {
            return Json(new
            {
                success = false,
                message,
                data = (object?)null
            });
        }

        private object MapCategory(UnitCategory x)
        {
            return new
            {
                unitCategoryID = x.UnitCategoryID,
                categoryName = x.CategoryName,
                isSystem = x.IsSystem,
                sort = x.Sort,
                createdBy = x.CreatedBy,
                createdDate = x.CreatedDate?.ToString("yyyy-MM-dd HH:mm"),
                updatedBy = x.UpdatedBy,
                updatedDate = x.UpdatedDate?.ToString("yyyy-MM-dd HH:mm")
            };
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                ViewBag.IsAdmin = IsAdmin;

                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesData(
            int page = 1,
            string search = "")
        {
            try
            {
                var result = await _categoryRepo.GetPagedAsync(
                    page,
                    10,
                    search);

                return JsonSuccess(
                    "Loaded successfully.",
                    new
                    {
                        items = result.Items.Select(MapCategory),
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages,
                        pageNumber = result.PageNumber,
                        pageSize = result.PageSize,
                        hasNextPage = result.HasNextPage,
                        hasPreviousPage = result.HasPreviousPage
                    });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryForEdit(int id)
        {
            try
            {
                var category = await _categoryRepo.GetByIdAsync(id);

                if (category == null)
                    return JsonFail("Category not found.");

                return JsonSuccess(
                    "Loaded successfully.",
                    new
                    {
                        unitCategoryID = category.UnitCategoryID,
                        categoryName = category.CategoryName,
                        isSystem = category.IsSystem,
                        sort = category.Sort
                    });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] UnitCategoryViewModel vm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vm.CategoryName))
                    return JsonFail("Category Name is required.");

                var model = new UnitCategory
                {
                    CategoryName = vm.CategoryName.Trim(),
                    Sort = vm.Sort,
                    IsSystem = false,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _categoryRepo.AddAsync(model);

                return JsonSuccess(
                    "Category created successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            [FromBody] UnitCategoryViewModel vm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vm.CategoryName))
                    return JsonFail("Category Name is required.");

                var existing = await _categoryRepo
                    .GetByIdAsync(vm.UnitCategoryID);

                if (existing == null)
                    return JsonFail("Category not found.");

                existing.Sort = vm.Sort;

                if (!existing.IsSystem)
                {
                    existing.CategoryName = vm.CategoryName.Trim();
                }

                existing.UpdatedBy = CurrentUsername;
                existing.UpdatedDate = DateTime.Now;

                await _categoryRepo.UpdateAsync(existing);

                return JsonSuccess(
                    "Category updated successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategories(
            [FromBody] DeleteCategoriesRequest request)
        {
            try
            {
                if (!IsAdmin)
                    return JsonFail("Unauthorized.");

                if (request == null ||
                    request.Ids == null ||
                    !request.Ids.Any())
                {
                    return JsonFail(
                        "No categories selected.");
                }

                foreach (var id in request.Ids)
                {
                    var category = await _categoryRepo
                        .GetByIdAsync(id);

                    if (category == null)
                        continue;

                    if (category.IsSystem)
                    {
                        return JsonFail(
                            "Cannot delete system categories.");
                    }

                    var count = await _categoryRepo
                        .GetUnitCountByCategoryAsync(id);

                    if (count > 0)
                    {
                        return JsonFail(
                            "Cannot delete — one or more categories have units assigned to them.");
                    }
                }

                await _categoryRepo.DeleteManyAsync(request.Ids);

                return JsonSuccess(
                    $"{request.Ids.Count} category(ies) deleted successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }
    }
}