using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Repositories;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IItemSubTypeRepository _itemSubTypeRepository;
        private readonly IUnitRepository _unitRepository;

        public ItemController(
            IItemRepository itemRepository,
            IItemTypeRepository itemTypeRepository,
            IItemSubTypeRepository itemSubTypeRepository,
            IUnitRepository unitRepository)
        {
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _itemSubTypeRepository = itemSubTypeRepository;
            _unitRepository = unitRepository;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int itemTypeFilter = 0,
            int itemSubTypeFilter = 0)
        {
            try
            {
                var itemTypes = await _itemTypeRepository.GetAllAsync();
                ViewData["ItemTypes"] = itemTypes;

                var result = await _itemRepository.GetPagedAsync(
                    page, 10, search, sortColumn, sortDirection,
                    itemTypeFilter,
                    itemSubTypeFilter,
                    IsAdmin);

                ViewData["Search"] = search;
                ViewData["SortColumn"] = sortColumn;
                ViewData["SortDirection"] = sortDirection;
                ViewData["ItemTypeFilter"] = itemTypeFilter;
                ViewData["ItemSubTypeFilter"] = itemSubTypeFilter;

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
                var itemTypes = await _itemTypeRepository.GetAllAsync();

                var viewModel = new ItemViewModel
                {
                    ItemTypeOptions = itemTypes.Select(it => new SelectListItem
                    {
                        Value = it.ItemTypeID.ToString(),
                        Text = it.ItemTypeName
                    }).ToList(),

                    UnitList = await GetUnitDropdown()
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
                    var itemTypes = await _itemTypeRepository.GetAllAsync();

                    viewModel.ItemTypeOptions = itemTypes.Select(it => new SelectListItem
                    {
                        Value = it.ItemTypeID.ToString(),
                        Text = it.ItemTypeName
                    }).ToList();

                    var subTypes = await _itemSubTypeRepository
                        .GetByItemTypeIdAsync(viewModel.ItemTypeID);

                    viewModel.SubTypeOptions = subTypes.Select(st => new SelectListItem
                    {
                        Value = st.ItemSubTypeID.ToString(),
                        Text = st.ItemSubTypeName
                    }).ToList();

                    viewModel.UnitList = await GetUnitDropdown();

                    return View(viewModel);
                }

                var item = new Item
                {
                    ItemName = viewModel.ItemName,
                    ItemTypeID = viewModel.ItemTypeID,
                    ItemSubTypeID = viewModel.ItemSubTypeID,

                    BaseUnitID = viewModel.BaseUnitID,
                    DisplayUnitID = viewModel.DisplayUnitID,

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

                var itemTypes = await _itemTypeRepository.GetAllAsync();

                var subTypes = await _itemSubTypeRepository
                    .GetByItemTypeIdAsync(item.ItemTypeID);

                var viewModel = new ItemViewModel
                {
                    ItemID = item.ItemID,
                    ItemName = item.ItemName,
                    ItemTypeID = item.ItemTypeID,
                    ItemSubTypeID = item.ItemSubTypeID,
                    Sort = item.Sort,

                    BaseUnitID = item.BaseUnitID,
                    DisplayUnitID = item.DisplayUnitID,

                    ItemTypeOptions = itemTypes.Select(it => new SelectListItem
                    {
                        Value = it.ItemTypeID.ToString(),
                        Text = it.ItemTypeName
                    }).ToList(),

                    SubTypeOptions = subTypes.Select(st => new SelectListItem
                    {
                        Value = st.ItemSubTypeID.ToString(),
                        Text = st.ItemSubTypeName
                    }).ToList(),

                    UnitList = await GetUnitDropdown()
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
                    var itemTypes = await _itemTypeRepository.GetAllAsync();
                    var subTypes = await _itemSubTypeRepository
                        .GetByItemTypeIdAsync(viewModel.ItemTypeID);

                    viewModel.ItemTypeOptions = itemTypes.Select(it => new SelectListItem
                    {
                        Value = it.ItemTypeID.ToString(),
                        Text = it.ItemTypeName
                    }).ToList();

                    viewModel.SubTypeOptions = subTypes.Select(st => new SelectListItem
                    {
                        Value = st.ItemSubTypeID.ToString(),
                        Text = st.ItemSubTypeName
                    }).ToList();

                    viewModel.UnitList = await GetUnitDropdown();

                    return View(viewModel);
                }

                var item = new Item
                {
                    ItemID = viewModel.ItemID,
                    ItemName = viewModel.ItemName,
                    ItemTypeID = viewModel.ItemTypeID,
                    ItemSubTypeID = viewModel.ItemSubTypeID,

                    BaseUnitID = viewModel.BaseUnitID,
                    DisplayUnitID = viewModel.DisplayUnitID,

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

        [HttpGet]
        public async Task<JsonResult> GetSubTypesByItemType(int itemTypeId)
        {
            try
            {
                var subTypes = await _itemSubTypeRepository.GetByItemTypeIdAsync(itemTypeId);

                var result = subTypes.Select(st => new
                {
                    value = st.ItemSubTypeID,
                    text = st.ItemSubTypeName
                });

                return Json(result);
            }
            catch (Exception)
            {
                return Json(new List<object>());
            }
        }

        private async Task<List<SelectListItem>> GetUnitDropdown()
        {
            var units = await _unitRepository.GetAllAsync();

            return units.Select(x => new SelectListItem
            {
                Value = x.UnitID.ToString(),
                Text = $"{x.UnitName} ({x.Abbreviation})"
            }).ToList();
        }

    }
}
