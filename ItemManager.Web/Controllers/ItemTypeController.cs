using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemTypeController : BaseController
    {
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemTypeController(IItemTypeRepository itemTypeRepository)
        {
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc")
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var result = await _itemTypeRepository.GetPagedAsync(
                    page, 10, search, sortColumn, sortDirection);

                // Pass current filter values back to View
                ViewData["Search"] = search;
                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortDirection;

                return View(result);
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
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var viewModel = new ItemTypeViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemTypeViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var itemType = new ItemType
                {
                    ItemTypeName = viewModel.ItemTypeName,
                    Sort = viewModel.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };
                await _itemTypeRepository.AddAsync(itemType);
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

                var itemType = await _itemTypeRepository.GetByIdAsync(id);
                if (itemType == null) return NotFound();

                var viewModel = new ItemTypeViewModel
                {
                    ItemTypeID = itemType.ItemTypeID,
                    ItemTypeName = itemType.ItemTypeName,
                    Sort = itemType.Sort
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
        public async Task<IActionResult> Edit(ItemTypeViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var itemType = new ItemType
                {
                    ItemTypeID = viewModel.ItemTypeID,
                    ItemTypeName = viewModel.ItemTypeName,
                    Sort = viewModel.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };
                await _itemTypeRepository.UpdateAsync(itemType);
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
