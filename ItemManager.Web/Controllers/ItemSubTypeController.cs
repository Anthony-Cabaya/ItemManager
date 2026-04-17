using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.Controllers
{
    public class ItemSubTypeController : BaseController
    {
        private readonly IItemSubTypeRepository _itemSubTypeRepository;
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemSubTypeController(
            IItemSubTypeRepository itemSubTypeRepository,
            IItemTypeRepository itemTypeRepository)
        {
            _itemSubTypeRepository = itemSubTypeRepository;
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<IActionResult> Index(int itemTypeFilter = 0)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                ViewBag.ItemTypes = await GetItemTypeDropdown();
                ViewBag.ItemTypeFilter = itemTypeFilter;

                IEnumerable<ItemSubType> subTypes =
                    itemTypeFilter > 0
                        ? await _itemSubTypeRepository.GetByItemTypeIdAsync(itemTypeFilter)
                        : await _itemSubTypeRepository.GetAllAsync();

                var viewModel = subTypes.Select(x => new ItemSubTypeViewModel
                {
                    ItemSubTypeID = x.ItemSubTypeID,
                    ItemSubTypeName = x.ItemSubTypeName,
                    ItemTypeID = x.ItemTypeID,
                    ItemTypeName = x.ItemTypeName,
                    Sort = x.Sort,
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

                var viewModel = new ItemSubTypeViewModel
                {
                    ItemTypeList = await GetItemTypeDropdown()
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
        public async Task<IActionResult> Create(ItemSubTypeViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                {
                    viewModel.ItemTypeList = await GetItemTypeDropdown();
                    return View(viewModel);
                }

                var model = new ItemSubType
                {
                    ItemSubTypeName = viewModel.ItemSubTypeName,
                    ItemTypeID = viewModel.ItemTypeID,
                    Sort = viewModel.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _itemSubTypeRepository.AddAsync(model);

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

                var viewModel = await GetById(id);
                if (viewModel == null)
                    return NotFound();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemSubTypeViewModel viewModel)
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction("Index", "Dashboard");

                if (!ModelState.IsValid)
                {
                    viewModel.ItemTypeList = await GetItemTypeDropdown();
                    return View(viewModel);
                }

                var model = new ItemSubType
                {
                    ItemSubTypeID = viewModel.ItemSubTypeID,
                    ItemSubTypeName = viewModel.ItemSubTypeName,
                    ItemTypeID = viewModel.ItemTypeID,
                    Sort = viewModel.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };

                await _itemSubTypeRepository.UpdateAsync(model);

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

                await _itemSubTypeRepository.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        private async Task<ItemSubTypeViewModel> GetById(int id)
        {
            var subType = await _itemSubTypeRepository.GetByIdAsync(id);
            if (subType == null) return null;

            return new ItemSubTypeViewModel
            {
                ItemSubTypeID = subType.ItemSubTypeID,
                ItemSubTypeName = subType.ItemSubTypeName,
                ItemTypeID = subType.ItemTypeID,
                Sort = subType.Sort,
                ItemTypeList = await GetItemTypeDropdown()
            };
        }

        private async Task<List<SelectListItem>> GetItemTypeDropdown()
        {
            var itemTypes = await _itemTypeRepository.GetAllAsync();

            return itemTypes.Select(x => new SelectListItem
            {
                Value = x.ItemTypeID.ToString(),
                Text = x.ItemTypeName
            }).ToList();
        }
    }
}