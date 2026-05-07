using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IItemRepository _itemRepo;
        private readonly IItemTypeRepository _itemTypeRepo;
        private readonly IItemSubTypeRepository _itemSubTypeRepo;
        private readonly IUnitRepository _unitRepo;
        private readonly IItemUnitConversionRepository _conversionRepo;
        private readonly IItemCodeService _itemCodeService;

        public ItemController(
            IItemRepository itemRepo,
            IItemTypeRepository itemTypeRepo,
            IItemSubTypeRepository itemSubTypeRepo,
            IUnitRepository unitRepo,
            IItemUnitConversionRepository conversionRepo,
            IItemCodeService itemCodeService)
        {
            _itemRepo = itemRepo;
            _itemTypeRepo = itemTypeRepo;
            _itemSubTypeRepo = itemSubTypeRepo;
            _unitRepo = unitRepo;
            _conversionRepo = conversionRepo;
            _itemCodeService = itemCodeService;
        }

        private JsonResult JsonSuccess(
            string message,
            object? data = null)
            => Json(new
            {
                success = true,
                message,
                data
            });

        private JsonResult JsonFail(string message)
            => Json(new
            {
                success = false,
                message,
                data = (object?)null
            });

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
                var itemTypes = await _itemTypeRepo.GetAllAsync();
                ViewData["ItemTypes"] = itemTypes;

                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetItemsData(
            int page = 1,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int itemTypeFilter = 0,
            int itemSubTypeFilter = 0)
        {
            try
            {
                const int pageSize = 10;

                var result = await _itemRepo.GetPagedAsync(
                    page,
                    pageSize,
                    search,
                    sortColumn,
                    sortDirection,
                    itemTypeFilter,
                    itemSubTypeFilter,
                    IsAdmin);

                var data = new
                {
                    items = result.Items.Select(x => new
                    {
                        itemID = x.ItemID,
                        itemCode = x.ItemCode,
                        itemName = x.ItemName,
                        sort = x.Sort,
                        itemTypeName = x.ItemType?.ItemTypeName,
                        itemSubTypeName = x.ItemSubTypeName,
                        baseUnitAbbreviation = x.BaseUnitAbbreviation,
                        condition = x.Condition,
                        createdBy = x.CreatedBy,
                        createdDate = x.CreatedDate?.ToString("yyyy-MM-dd HH:mm"),
                        updatedBy = x.UpdatedBy,
                        updatedDate = x.UpdatedDate?.ToString("yyyy-MM-dd HH:mm"),
                        baseUnitID = x.BaseUnitID,
                        variants = "—",
                        currentStock = "—",
                        unitCost = "—"
                    }),

                    totalCount = result.TotalCount,
                    totalPages = result.TotalPages,
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize,
                    hasNextPage = result.HasNextPage,
                    hasPreviousPage = result.HasPreviousPage
                };

                return JsonSuccess("Items loaded successfully.", data);
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetItemForEdit(int id)
        {
            try
            {
                var item = await _itemRepo.GetByIdAsync(id);

                if (item == null)
                    return JsonFail("Item not found.");

                var itemTypes = await _itemTypeRepo.GetAllAsync();

                var subTypes = await _itemSubTypeRepo
                    .GetByItemTypeIdAsync(item.ItemTypeID);

                IEnumerable<Unit> units;

                if (item.BaseUnitID.HasValue)
                {
                    var baseUnit = await _unitRepo
                        .GetByIdAsync(item.BaseUnitID.Value);

                    if (baseUnit != null)
                    {
                        units = await _unitRepo
                            .GetByCategoryIdAsync(baseUnit.UnitCategoryID);
                    }
                    else
                    {
                        units = await _unitRepo.GetAllAsync();
                    }
                }
                else
                {
                    units = await _unitRepo.GetAllAsync();
                }

                return JsonSuccess(
                    "Item loaded successfully.",
                    new
                    {
                        itemID = item.ItemID,
                        itemCode = item.ItemCode,
                        itemName = item.ItemName,
                        sort = item.Sort,
                        itemTypeID = item.ItemTypeID,
                        itemSubTypeID = item.ItemSubTypeID,
                        baseUnitID = item.BaseUnitID,
                        displayUnitID = item.DisplayUnitID,
                        condition = item.Condition,

                        itemTypeOptions = itemTypes.Select(x => new
                        {
                            value = x.ItemTypeID,
                            text = x.ItemTypeName
                        }),

                        subTypeOptions = subTypes.Select(x => new
                        {
                            value = x.ItemSubTypeID,
                            text = x.ItemSubTypeName
                        }),

                        unitOptions = units.Select(x => new
                        {
                            value = x.UnitID,
                            text = $"{x.UnitName} ({x.Abbreviation})"
                        })
                    });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGeneratedCode(
            int itemTypeId,
            int? itemSubTypeId)
        {
            try
            {
                var itemTypes = await _itemTypeRepo.GetAllAsync();
                var itemType = itemTypes.FirstOrDefault(x => x.ItemTypeID == itemTypeId);

                if (itemType == null)
                    return JsonFail("Invalid Item Type.");

                string? subTypeName = null;

                if (itemSubTypeId.HasValue)
                {
                    var subTypes = await _itemSubTypeRepo.GetByItemTypeIdAsync(itemTypeId);
                    subTypeName = subTypes.FirstOrDefault(x => x.ItemSubTypeID == itemSubTypeId)?.ItemSubTypeName;
                }

                var code = await _itemCodeService.PreviewCodeAsync(
                    itemTypeId,
                    itemSubTypeId,
                    itemType.ItemTypeName!,
                    subTypeName);

                return JsonSuccess("Code generated successfully.", new { code });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create(ItemViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault()
                        ?? "Validation failed.";

                    return JsonFail(firstError);
                }

                var itemTypes = await _itemTypeRepo.GetAllAsync();

                var itemType = itemTypes
                    .FirstOrDefault(x => x.ItemTypeID == vm.ItemTypeID);

                if (itemType == null)
                    return JsonFail("Invalid Item Type.");

                string? subTypeName = null;

                if (vm.ItemSubTypeID.HasValue)
                {
                    var subTypes = await _itemSubTypeRepo
                        .GetByItemTypeIdAsync(vm.ItemTypeID);

                    subTypeName = subTypes
                        .FirstOrDefault(x =>
                            x.ItemSubTypeID == vm.ItemSubTypeID)
                        ?.ItemSubTypeName;
                }

                var generatedCode =
                    await _itemCodeService.GenerateCodeAsync(
                        vm.ItemTypeID,
                        vm.ItemSubTypeID,
                        itemType.ItemTypeName!,
                        subTypeName);

                string finalCode = generatedCode;

                if (!string.IsNullOrWhiteSpace(vm.ItemCode)
                    && vm.ItemCode != generatedCode)
                {
                    var isUnique = await _itemCodeService
                        .IsCodeUniqueAsync(vm.ItemCode);

                    if (!isUnique)
                    {
                        return JsonFail(
                            "Item Code already exists.");
                    }

                    finalCode = vm.ItemCode;
                }

                var item = new Item
                {
                    ItemName = vm.ItemName,
                    ItemTypeID = vm.ItemTypeID,
                    ItemSubTypeID = vm.ItemSubTypeID,
                    ItemCode = finalCode,
                    Condition = vm.Condition,
                    BaseUnitID = vm.BaseUnitID,
                    DisplayUnitID = vm.DisplayUnitID,
                    Sort = vm.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _itemRepo.AddAsync(item);

                return JsonSuccess(
                    "Item created successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Update(ItemViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault()
                        ?? "Validation failed.";

                    return JsonFail(firstError);
                }

                var existing = await _itemRepo.GetByIdAsync(vm.ItemID);

                if (existing == null)
                    return JsonFail("Item not found.");

                bool baseUnitChanged =
                    existing.BaseUnitID != vm.BaseUnitID;

                if (baseUnitChanged)
                {
                    var conversions = await _conversionRepo
                        .GetByItemIdAsync(vm.ItemID);

                    if (conversions.Any())
                    {
                        return JsonFail(
                            "Cannot change Base Unit while conversions exist. Delete all conversions first.");
                    }
                }

                if (!string.Equals(
                    existing.ItemCode,
                    vm.ItemCode,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var isUnique = await _itemCodeService
                        .IsCodeUniqueAsync(vm.ItemCode ?? "");

                    if (!isUnique)
                    {
                        return JsonFail(
                            "Item Code already exists.");
                    }
                }

                existing.ItemName = vm.ItemName;
                existing.ItemTypeID = vm.ItemTypeID;
                existing.ItemSubTypeID = vm.ItemSubTypeID;
                existing.ItemCode = vm.ItemCode;
                existing.Condition = vm.Condition;
                existing.BaseUnitID = vm.BaseUnitID;
                existing.DisplayUnitID = vm.DisplayUnitID;
                existing.Sort = vm.Sort;
                existing.UpdatedBy = CurrentUsername;
                existing.UpdatedDate = DateTime.Now;

                await _itemRepo.UpdateAsync(existing);

                return JsonSuccess(
                    "Item updated successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteItems(
            List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return JsonFail(
                        "No items selected.");
                }

                if (!IsAdmin)
                {
                    return JsonFail(
                        "Unauthorized.");
                }

                foreach (var id in ids)
                {
                    await _itemRepo.DeleteAsync(id);
                }

                return JsonSuccess(
                    $"{ids.Count} item(s) deleted successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUnitsForItem(
            int unitCategoryId)
        {
            try
            {
                var units = await _unitRepo
                    .GetByCategoryIdAsync(unitCategoryId);

                var result = units.Select(x => new
                {
                    value = x.UnitID,
                    text = $"{x.UnitName} ({x.Abbreviation})"
                });

                return JsonSuccess(
                    "Units loaded successfully.",
                    result);
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSubTypesByItemType(
            int itemTypeId)
        {
            try
            {
                var subTypes = await _itemSubTypeRepo
                    .GetByItemTypeIdAsync(itemTypeId);

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

        [HttpGet]
        public async Task<IActionResult> Conversions(int id)
        {
            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            if (!item.BaseUnitID.HasValue)
            {
                TempData["ErrorMessage"] =
                    "Please set a Base Unit for this item first.";

                return RedirectToAction("Index");
            }

            var conversions =
                await _conversionRepo.GetByItemIdAsync(id);

            var baseUnit =
                await _unitRepo.GetByIdAsync(
                    item.BaseUnitID.Value);

            var allUnits =
                await _unitRepo.GetAllAsync();

            var usedUnitIds =
                conversions.Select(x => x.UnitID).ToList();

            var availableUnits = allUnits
                .Where(u => u.UnitID != item.BaseUnitID
                         && !usedUnitIds.Contains(u.UnitID))
                .Select(u => new SelectListItem
                {
                    Value = u.UnitID.ToString(),
                    Text = $"{u.UnitName} ({u.Abbreviation})"
                }).ToList();

            var vm = new ItemUnitConversionViewModel
            {
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                BaseUnitName = baseUnit?.UnitName,
                BaseUnitAbbreviation = baseUnit?.Abbreviation,
                Conversions = conversions,
                AvailableUnits = availableUnits
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddConversion(
            ItemUnitConversionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var item = await _itemRepo.GetByIdAsync(vm.ItemID);

                if (item == null)
                    return NotFound();

                var conversions =
                    await _conversionRepo
                        .GetByItemIdAsync(vm.ItemID);

                var baseUnit =
                    await _unitRepo
                        .GetByIdAsync(item.BaseUnitID!.Value);

                var allUnits =
                    await _unitRepo.GetAllAsync();

                var usedUnitIds =
                    conversions.Select(x => x.UnitID).ToList();

                vm.ItemName = item.ItemName;
                vm.BaseUnitName = baseUnit?.UnitName;
                vm.BaseUnitAbbreviation = baseUnit?.Abbreviation;
                vm.Conversions = conversions;

                vm.AvailableUnits = allUnits
                    .Where(u => u.UnitID != item.BaseUnitID
                             && !usedUnitIds.Contains(u.UnitID))
                    .Select(u => new SelectListItem
                    {
                        Value = u.UnitID.ToString(),
                        Text = $"{u.UnitName} ({u.Abbreviation})"
                    }).ToList();

                return View("Conversions", vm);
            }

            var model = new ItemUnitConversion
            {
                ItemID = vm.ItemID,
                UnitID = vm.UnitID,
                Factor = vm.Factor,
                CreatedBy = CurrentUsername,
                CreatedDate = DateTime.Now
            };

            await _conversionRepo.AddAsync(model);

            return RedirectToAction(
                "Conversions",
                new { id = vm.ItemID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConversion(
            int conversionId,
            int itemId)
        {
            await _conversionRepo.DeleteAsync(conversionId);

            return RedirectToAction(
                "Conversions",
                new { id = itemId });
        }

    }
}