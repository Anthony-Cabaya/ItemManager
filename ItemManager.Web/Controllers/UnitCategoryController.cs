using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class UnitCategoryController : BaseController
    {
        private readonly IUnitCategoryRepository _unitCategoryRepository;

        public UnitCategoryController(IUnitCategoryRepository unitCategoryRepository)
        {
            _unitCategoryRepository = unitCategoryRepository;
        }

        public async Task<IActionResult> Index(
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc")
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                ViewData["Search"] = search;
                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortDirection;

                IEnumerable<UnitCategory> categories = await _unitCategoryRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();

                    categories = categories.Where(x =>
                        !string.IsNullOrEmpty(x.CategoryName) &&
                        x.CategoryName.ToLower().Contains(search)
                    );
                }

                categories = sortColumn switch
                {
                    "CategoryName" => sortDirection == "asc"
                        ? categories.OrderBy(x => x.CategoryName)
                        : categories.OrderByDescending(x => x.CategoryName),

                    "Sort" => sortDirection == "asc"
                        ? categories.OrderBy(x => x.Sort)
                        : categories.OrderByDescending(x => x.Sort),

                    _ => categories.OrderBy(x => x.Sort)
                };

                var viewModel = categories.Select(x => new UnitCategoryViewModel
                {
                    UnitCategoryID = x.UnitCategoryID,
                    CategoryName = x.CategoryName,
                    Sort = x.Sort,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    IsSystem = x.IsSystem
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin)
                return RedirectToAction("Index", "Dashboard");

            return View(new UnitCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UnitCategoryViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                    return View(viewModel);

                var model = new UnitCategory
                {
                    CategoryName = viewModel.CategoryName,
                    Sort = viewModel.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _unitCategoryRepository.AddAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var category = await _unitCategoryRepository.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                if (category.IsSystem)
                {
                    TempData["ErrorMessage"] = "System categories cannot be edited.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new UnitCategoryViewModel
                {
                    UnitCategoryID = category.UnitCategoryID,
                    CategoryName = category.CategoryName,
                    Sort = category.Sort,
                    IsSystem = category.IsSystem
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UnitCategoryViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var existing = await _unitCategoryRepository.GetByIdAsync(viewModel.UnitCategoryID);
                if (existing == null)
                    return NotFound();

                if (existing.IsSystem)
                {
                    TempData["ErrorMessage"] = "System categories cannot be edited.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                    return View(viewModel);

                var model = new UnitCategory
                {
                    UnitCategoryID = viewModel.UnitCategoryID,
                    CategoryName = viewModel.CategoryName,
                    Sort = viewModel.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now,
                    IsSystem = existing.IsSystem
                };

                await _unitCategoryRepository.UpdateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var category = await _unitCategoryRepository.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                if (category.IsSystem)
                {
                    TempData["ErrorMessage"] = "System categories cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await _unitCategoryRepository.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

    }
}
