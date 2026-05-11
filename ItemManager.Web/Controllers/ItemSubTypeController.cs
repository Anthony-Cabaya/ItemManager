using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemSubTypeController : BaseController
    {
        private const int PageSize = 10;

        private readonly IItemSubTypeRepository _subTypeRepo;
        private readonly IItemTypeRepository _itemTypeRepo;

        public ItemSubTypeController(
            IItemSubTypeRepository subTypeRepo,
            IItemTypeRepository itemTypeRepo)
        {
            _subTypeRepo = subTypeRepo;
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

        private object MapSubType(ItemSubType x)
        {
            return new
            {
                itemSubTypeID = x.ItemSubTypeID,
                itemSubTypeName = x.ItemSubTypeName,
                itemTypeName = x.ItemTypeName,
                itemTypeID = x.ItemTypeID,
                sort = x.Sort,

                createdBy = x.CreatedBy,

                createdDate = x.CreatedDate?.ToString(
                    "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture),

                updatedBy = x.UpdatedBy,

                updatedDate = x.UpdatedDate.HasValue
                    ? x.UpdatedDate.Value.ToString(
                        "yyyy-MM-dd HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture)
                    : null
            };
        }

        public class DeleteSubTypesRequest
        {
            public List<int> Ids { get; set; } = new();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (!IsAdmin)
                    return RedirectToAction(
                        "Index",
                        "Dashboard");

                var itemTypes =
                    await _itemTypeRepo.GetAllAsync();

                ViewBag.ItemTypes = itemTypes;
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
        public async Task<IActionResult> GetSubTypesData(
            int page = 1,
            string search = "",
            int itemTypeFilter = 0)
        {
            try
            {
                var result =
                    await _subTypeRepo.GetPagedAsync(
                        page,
                        PageSize,
                        search,
                        itemTypeFilter);

                return JsonSuccess(
                    "Loaded successfully.",
                    new
                    {
                        items = result.Items.Select(MapSubType),
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
        public async Task<IActionResult> GetSubTypeForEdit(
            int id)
        {
            try
            {
                var subType =
                    await _subTypeRepo.GetByIdAsync(id);

                if (subType == null)
                {
                    return JsonFail(
                        "Sub Type not found.");
                }

                return JsonSuccess(
                    "Loaded successfully.",
                    new
                    {
                        itemSubTypeID = subType.ItemSubTypeID,
                        itemSubTypeName = subType.ItemSubTypeName,
                        itemTypeID = subType.ItemTypeID,
                        sort = subType.Sort
                    });
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult>
            GetItemTypesForDropdown()
        {
            try
            {
                var itemTypes =
                    await _itemTypeRepo.GetAllAsync();

                return Json(
                    itemTypes.Select(x => new
                    {
                        value = x.ItemTypeID,
                        text = x.ItemTypeName
                    }));
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ItemSubTypeViewModel vm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    vm.ItemSubTypeName))
                {
                    return JsonFail(
                        "Sub Type Name is required.");
                }

                if (vm.ItemTypeID <= 0)
                {
                    return JsonFail(
                        "Please select an Item Type.");
                }

                var model = new ItemSubType
                {
                    ItemSubTypeName =
                        vm.ItemSubTypeName.Trim(),

                    ItemTypeID = vm.ItemTypeID,
                    Sort = vm.Sort,

                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _subTypeRepo.AddAsync(model);

                return JsonSuccess(
                    "Sub Type created successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            [FromBody] ItemSubTypeViewModel vm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    vm.ItemSubTypeName))
                {
                    return JsonFail(
                        "Sub Type Name is required.");
                }

                if (vm.ItemTypeID <= 0)
                {
                    return JsonFail(
                        "Please select an Item Type.");
                }

                var existing =
                    await _subTypeRepo.GetByIdAsync(
                        vm.ItemSubTypeID);

                if (existing == null)
                {
                    return JsonFail(
                        "Sub Type not found.");
                }

                existing.ItemSubTypeName =
                    vm.ItemSubTypeName.Trim();

                existing.ItemTypeID =
                    vm.ItemTypeID;

                existing.Sort =
                    vm.Sort;

                existing.UpdatedBy =
                    CurrentUsername;

                existing.UpdatedDate =
                    DateTime.Now;

                await _subTypeRepo.UpdateAsync(
                    existing);

                return JsonSuccess(
                    "Sub Type updated successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubTypes(
            [FromBody] DeleteSubTypesRequest request)
        {
            try
            {
                if (!IsAdmin)
                {
                    return JsonFail(
                        "Unauthorized.");
                }

                if (request.Ids == null ||
                    !request.Ids.Any())
                {
                    return JsonFail(
                        "No Sub Types selected.");
                }

                foreach (var id in request.Ids)
                {
                    var count =
                        await _subTypeRepo
                            .GetItemCountBySubTypeAsync(id);

                    if (count > 0)
                    {
                        return JsonFail(
                            "Cannot delete — one or more Sub Types have items assigned to them.");
                    }
                }

                await _subTypeRepo.DeleteManyAsync(
                    request.Ids);

                return JsonSuccess(
                    $"{request.Ids.Count} sub type(s) deleted successfully.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }
    }
}