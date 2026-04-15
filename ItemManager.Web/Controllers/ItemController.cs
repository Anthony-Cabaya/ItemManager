using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemController(IItemRepository itemRepository, IItemTypeRepository itemTypeRepository)
        {
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int itemTypeFilter = 0)
        {
            try
            {
                var itemTypes = await _itemTypeRepository.GetAllAsync();
                ViewData["ItemTypes"] = itemTypes;

                var result = await _itemRepository.GetPagedAsync(
                    page, 10, search, sortColumn, sortDirection, itemTypeFilter);

                ViewData["Search"] = search;
                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortDirection;
                ViewData["ItemTypeFilter"] = itemTypeFilter;

                return View(result);
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
                var viewModel = new ItemViewModel
                {
                    ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList()
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
        public async Task<IActionResult> Create(ItemViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList();
                    return View(viewModel);
                }

                var item = new Item
                {
                    ItemName = viewModel.ItemName,
                    ItemTypeID = viewModel.ItemTypeID,
                    Sort = viewModel.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };
                await _itemRepository.AddAsync(item);
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
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null) return NotFound();

                var viewModel = new ItemViewModel
                {
                    ItemID = item.ItemID,
                    ItemName = item.ItemName,
                    ItemTypeID = item.ItemTypeID,
                    Sort = item.Sort,
                    ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList()
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
        public async Task<IActionResult> Edit(ItemViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList();
                    return View(viewModel);
                }

                var item = new Item
                {
                    ItemID = viewModel.ItemID,
                    ItemName = viewModel.ItemName,
                    ItemTypeID = viewModel.ItemTypeID,
                    Sort = viewModel.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };
                await _itemRepository.UpdateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null) return NotFound();

                return View(item);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                await _itemRepository.DeleteAsync(id);
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
