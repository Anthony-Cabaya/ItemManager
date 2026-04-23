using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.Controllers
{
    public class UnitController : BaseController
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IUnitCategoryRepository _unitCategoryRepository;

        public UnitController(
            IUnitRepository unitRepository,
            IUnitCategoryRepository unitCategoryRepository)
        {
            _unitRepository = unitRepository;
            _unitCategoryRepository = unitCategoryRepository;
        }

        public async Task<IActionResult> Index(
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int categoryFilter = 0)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                ViewData["Search"] = search;
                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortDirection;
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.Categories = await GetCategoryDropdown();

                IEnumerable<Unit> units =
                    categoryFilter > 0
                        ? await _unitRepository.GetByCategoryIdAsync(categoryFilter)
                        : await _unitRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    units = units.Where(x =>
                        (!string.IsNullOrEmpty(x.UnitName) && x.UnitName.ToLower().Contains(search))
                    );
                }

                units = sortColumn switch
                {
                    "UnitName" => sortDirection == "asc"
                        ? units.OrderBy(x => x.UnitName)
                        : units.OrderByDescending(x => x.UnitName),

                    "Sort" => sortDirection == "asc"
                        ? units.OrderBy(x => x.Sort)
                        : units.OrderByDescending(x => x.Sort),

                    _ => units.OrderBy(x => x.Sort)
                };

                var viewModel = units.Select(x => new UnitViewModel
                {
                    UnitID = x.UnitID,
                    UnitName = x.UnitName,
                    Abbreviation = x.Abbreviation,
                    UnitCategoryID = x.UnitCategoryID,
                    UnitCategoryName = x.UnitCategoryName,
                    Sort = x.Sort,
                    IsSystem = x.IsSystem,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
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
        public async Task<IActionResult> Create()
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var viewModel = new UnitViewModel
                {
                    UnitCategoryList = await GetCategoryDropdown()
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
        public async Task<IActionResult> Create(UnitViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                {
                    viewModel.UnitCategoryList = await GetCategoryDropdown();
                    return View(viewModel);
                }

                var model = new Unit
                {
                    UnitName = viewModel.UnitName,
                    Abbreviation = viewModel.Abbreviation,
                    UnitCategoryID = viewModel.UnitCategoryID,
                    Sort = viewModel.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _unitRepository.AddAsync(model);

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

                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null)
                    return NotFound();

                var viewModel = new UnitViewModel
                {
                    UnitID = unit.UnitID,
                    UnitName = unit.UnitName,
                    Abbreviation = unit.Abbreviation,
                    UnitCategoryID = unit.UnitCategoryID,
                    UnitCategoryName = unit.UnitCategoryName,
                    Sort = unit.Sort,
                    IsSystem = unit.IsSystem,
                    UnitCategoryList = await GetCategoryDropdown()
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
        public async Task<IActionResult> Edit(UnitViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var existing = await _unitRepository.GetByIdAsync(viewModel.UnitID);
                if (existing == null)
                    return NotFound();

                if (existing.IsSystem)
                {
                    ModelState.Remove("UnitName");
                    ModelState.Remove("Abbreviation");
                    ModelState.Remove("UnitCategoryID");
                }

                if (!ModelState.IsValid)
                {
                    viewModel.UnitCategoryList = await GetCategoryDropdown();
                    return View(viewModel);
                }

                Unit model;

                if (existing.IsSystem)
                {
                    // Only allow Sort update
                    model = new Unit
                    {
                        UnitID = existing.UnitID,
                        UnitName = existing.UnitName,
                        Abbreviation = existing.Abbreviation,
                        UnitCategoryID = existing.UnitCategoryID,
                        Sort = viewModel.Sort,
                        IsSystem = existing.IsSystem,
                        UpdatedBy = CurrentUsername,
                        UpdatedDate = DateTime.Now
                    };
                }
                else
                {
                    // Full update
                    model = new Unit
                    {
                        UnitID = viewModel.UnitID,
                        UnitName = viewModel.UnitName,
                        Abbreviation = viewModel.Abbreviation,
                        UnitCategoryID = viewModel.UnitCategoryID,
                        Sort = viewModel.Sort,
                        IsSystem = existing.IsSystem,
                        UpdatedBy = CurrentUsername,
                        UpdatedDate = DateTime.Now
                    };
                }

                await _unitRepository.UpdateAsync(model);

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

                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null)
                    return NotFound();

                if (unit.IsSystem)
                {
                    TempData["ErrorMessage"] = "System units cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await _unitRepository.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        private async Task<List<SelectListItem>> GetCategoryDropdown()
        {
            var categories = await _unitCategoryRepository.GetAllAsync();

            return categories.Select(x => new SelectListItem
            {
                Value = x.UnitCategoryID.ToString(),
                Text = x.CategoryName
            }).ToList();
        }

    }
}
