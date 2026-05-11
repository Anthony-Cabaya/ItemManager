using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemTypeController : BaseController
    {
        private const int PageSize = 10;

        private readonly IItemTypeRepository _itemTypeRepo;

        public ItemTypeController(IItemTypeRepository itemTypeRepo)
        {
            _itemTypeRepo = itemTypeRepo;
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

        private JsonResult? ValidateItemType(ItemTypeViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ItemTypeName))
            {
                return JsonFail(
                    "Item Type Name is required.");
            }

            return null;
        }

        private async Task<bool> HasAssignedItemsAsync(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var count =
                    await _itemTypeRepo.GetItemCountByTypeAsync(id);

                if (count > 0)
                    return true;
            }

            return false;
        }

        private object MapItemType(ItemType itemType)
        {
            return new
            {
                itemTypeID = itemType.ItemTypeID,
                itemTypeName = itemType.ItemTypeName,
                sort = itemType.Sort,
                createdBy = itemType.CreatedBy,
                createdDate = itemType.CreatedDate?.ToString("yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture),
                updatedBy = itemType.UpdatedBy,
                updatedDate = itemType.UpdatedDate.HasValue
                    ? itemType.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture)
                    : null
            };
        }
        private void ApplyCreateAudit(ItemType itemType)
        {
            itemType.CreatedBy = CurrentUsername;
            itemType.CreatedDate = DateTime.Now;
        }

        private void ApplyUpdateAudit(ItemType itemType)
        {
            itemType.UpdatedBy = CurrentUsername;
            itemType.UpdatedDate = DateTime.Now;
        }

        public class DeleteItemTypesRequest
        {
            public List<int> Ids { get; set; } = new();
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
        public async Task<IActionResult> GetItemTypesData(
            int page = 1,
            string search = "")
        {
            try
            {
                var result = await _itemTypeRepo.GetPagedAsync(
                    page,
                    PageSize,
                    search);

                return JsonSuccess("Loaded successfully.", new
                {
                    items = result.Items.Select(MapItemType),
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
        public async Task<IActionResult> GetItemTypeForEdit(int id)
        {
            try
            {
                var itemType =
                    await _itemTypeRepo.GetByIdAsync(id);

                if (itemType == null)
                    return JsonFail("Item Type not found.");

                return JsonSuccess("Loaded successfully.", new
                {
                    itemTypeID = itemType.ItemTypeID,
                    itemTypeName = itemType.ItemTypeName,
                    sort = itemType.Sort
                });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ItemTypeViewModel vm)
        {
            try
            {
                var validation = ValidateItemType(vm);

                if (validation != null)
                    return validation;

                var itemType = new ItemType
                {
                    ItemTypeName = vm.ItemTypeName.Trim(),
                    Sort = vm.Sort
                };

                ApplyCreateAudit(itemType);

                await _itemTypeRepo.AddAsync(itemType);

                return JsonSuccess(
                    "Item Type created successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            [FromBody] ItemTypeViewModel vm)
        {
            try
            {
                var validation = ValidateItemType(vm);
                if (validation != null)
                    return validation;

                var existing =
                    await _itemTypeRepo.GetByIdAsync(vm.ItemTypeID);

                if (existing == null)
                    return JsonFail("Item Type not found.");

                existing.ItemTypeName = vm.ItemTypeName.Trim();
                existing.Sort = vm.Sort;

                ApplyUpdateAudit(existing);

                await _itemTypeRepo.UpdateAsync(existing);

                return JsonSuccess(
                    "Item Type updated successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItemTypes(
            [FromBody] DeleteItemTypesRequest request)
        {
            try
            {
                if (!IsAdmin)
                    return JsonFail("Unauthorized.");

                if (request.Ids == null || !request.Ids.Any())
                    return JsonFail("No Item Types selected.");

                if (await HasAssignedItemsAsync(request.Ids))
                    return JsonFail("Cannot delete — one or more Item Types have items assigned to them.");

                await _itemTypeRepo.DeleteManyAsync(
                    request.Ids);

                return JsonSuccess(
                    $"{request.Ids.Count} item type(s) deleted successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }
    }
}