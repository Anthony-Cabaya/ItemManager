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

        // ---------------- DRY HELPERS ----------------
        private IActionResult? AdminOnly()
        {
            if (!IsAdmin)
                return RedirectToAction("Index", "Dashboard");

            return null;
        }

        private UnitViewModel Map(Unit x) => new()
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
        };

        private async Task<List<SelectListItem>> GetCategoryDropdownAsync()
        {
            var categories = await _unitCategoryRepository.GetAllAsync();

            return categories.Select(x => new SelectListItem
            {
                Value = x.UnitCategoryID.ToString(),
                Text = x.CategoryName
            }).ToList();
        }

        private IEnumerable<Unit> ApplySearch(IEnumerable<Unit> units, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return units;

            search = search.Trim().ToLower();

            return units.Where(x =>
                !string.IsNullOrEmpty(x.UnitName) &&
                x.UnitName.ToLower().Contains(search));
        }

        private bool IsAjaxRequest() =>
            Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        public async Task<IActionResult> Index()
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                var categories = await _unitCategoryRepository.GetAllAsync();

                var categoryVms = categories.Select(c => new UnitCategoryViewModel
                {
                    UnitCategoryID = c.UnitCategoryID,
                    CategoryName = c.CategoryName,
                    Sort = c.Sort,
                    IsSystem = c.IsSystem,
                    CreatedBy = c.CreatedBy,
                    CreatedDate = c.CreatedDate,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedDate = c.UpdatedDate
                }).ToList();

                return View(categoryVms);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl = "")
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                ViewData["ReturnUrl"] = returnUrl;

                return View(new UnitViewModel
                {
                    UnitCategoryList = await GetCategoryDropdownAsync()
                });
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(UnitViewModel vm, string returnUrl = "")
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                if (!ModelState.IsValid)
                {
                    if (IsAjaxRequest())
                        return Json(new { success = false, message = "Please fill in all required fields." });

                    ViewData["ReturnUrl"] = returnUrl;
                    vm.UnitCategoryList = await GetCategoryDropdownAsync();
                    return View(vm);
                }

                await _unitRepository.AddAsync(new Unit
                {
                    UnitName = vm.UnitName,
                    Abbreviation = vm.Abbreviation,
                    UnitCategoryID = vm.UnitCategoryID,
                    Sort = vm.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                });

                if (IsAjaxRequest())
                    return Json(new { success = true, message = "Unit created." });

                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction(nameof(Index))
                    : Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        // ---------------- EDIT ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl = "")
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null) return NotFound();

                ViewData["ReturnUrl"] = returnUrl;

                var vm = Map(unit);
                vm.UnitCategoryList = await GetCategoryDropdownAsync();

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UnitViewModel vm, string returnUrl = "")
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                var existing = await _unitRepository.GetByIdAsync(vm.UnitID);
                if (existing == null) return NotFound();

                var isSystem = existing.IsSystem;

                if (isSystem)
                {
                    ModelState.Remove(nameof(vm.UnitName));
                    ModelState.Remove(nameof(vm.Abbreviation));
                    ModelState.Remove(nameof(vm.UnitCategoryID));
                }

                if (!ModelState.IsValid)
                {
                    if (IsAjaxRequest())
                        return Json(new { success = false, message = "Please fill in all required fields." });

                    ViewData["ReturnUrl"] = returnUrl;
                    vm.UnitCategoryList = await GetCategoryDropdownAsync();
                    return View(vm);
                }

                var model = new Unit
                {
                    UnitID = vm.UnitID,
                    UnitName = isSystem ? existing.UnitName : vm.UnitName,
                    Abbreviation = isSystem ? existing.Abbreviation : vm.Abbreviation,
                    UnitCategoryID = isSystem ? existing.UnitCategoryID : vm.UnitCategoryID,
                    Sort = vm.Sort,
                    IsSystem = isSystem,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };

                await _unitRepository.UpdateAsync(model);

                if (IsAjaxRequest())
                    return Json(new { success = true, message = "Unit updated." });

                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction(nameof(Index))
                    : Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        // ---------------- DELETE (FIXED) ----------------
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var redirect = AdminOnly();
                if (redirect != null) return redirect;

                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null) return NotFound();

                if (unit.IsSystem)
                    return Json(new { success = false, message = "System units cannot be deleted." });

                if (await _unitRepository.HasItemsUsingUnitAsync(id))
                    return Json(new { success = false, message = "Cannot delete — unit is used by existing items." });

                await _unitRepository.DeleteAsync(id);

                if (IsAjaxRequest())
                    return Json(new { success = true, message = "Unit deleted." });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ---------------- PARTIAL ----------------
        [HttpGet]
        public async Task<IActionResult> GetUnitsByCategory(int categoryId, string returnUrl = "")
        {
            try
            {
                var units = await _unitRepository.GetByCategoryIdAsync(categoryId);

                var vm = units.Select(Map).ToList();

                ViewData["ReturnUrl"] = returnUrl;

                return PartialView("_UnitTablePartial", vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}