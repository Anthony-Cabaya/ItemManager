using ItemManager.Core.Interfaces;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ItemManager.Web.Controllers
{
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILocationRepository _locationRepository;
        private readonly IItemStockRepository _itemStockRepository;

        public InventoryController(
            IInventoryService inventoryService,
            ILocationRepository locationRepository,
            IItemStockRepository itemStockRepository)
        {
            _inventoryService = inventoryService;
            _locationRepository = locationRepository;
            _itemStockRepository = itemStockRepository;
        }

        public class SetStockRequest
        {
            public int ItemId { get; set; }
            public int LocationId { get; set; }
            public decimal Quantity { get; set; }
            public decimal? MinStock { get; set; }
            public int? ItemVariantId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? itemId = null, int? locationId = null)
        {
            try
            {
                ViewBag.Locations = await _locationRepository.GetAllAsync();
                ViewBag.ItemID = itemId;
                ViewBag.LocationID = locationId;

                return View();
            }
            catch (SqlException ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByItem(int itemId)
        {
            try
            {
                var result = await _inventoryService.GetStockByItemAsync(itemId);

                var model = result.Select(x => new ItemStockViewModel
                {
                    StockID = x.StockID,
                    ItemID = x.ItemID,
                    LocationID = x.LocationID,
                    ItemVariantID = x.ItemVariantID,
                    VariantName = x.VariantName,
                    Quantity = x.Quantity,
                    MinStock = x.MinStock,
                    LastUpdated = x.LastUpdated,
                    ItemName = x.ItemName,
                    ItemCode = x.ItemCode,
                    LocationName = x.LocationName,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();

                return PartialView("Partials/_StockTablePartial", model);
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            try
            {
                var result = await _inventoryService.GetStockByLocationAsync(locationId);

                var model = result.Select(x => new ItemStockViewModel
                {
                    StockID = x.StockID,
                    ItemID = x.ItemID,
                    LocationID = x.LocationID,
                    ItemVariantID = x.ItemVariantID,
                    VariantName = x.VariantName,
                    Quantity = x.Quantity,
                    MinStock = x.MinStock,
                    LastUpdated = x.LastUpdated,
                    ItemName = x.ItemName,
                    ItemCode = x.ItemCode,
                    LocationName = x.LocationName,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();

                return PartialView("Partials/_StockTablePartial", model);
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStock(
            [FromBody] SetStockRequest request)
        {
            try
            {
                await _inventoryService.SetStockAsync(
                    request.ItemId,
                    request.LocationId,
                    request.Quantity,
                    request.MinStock,
                    CurrentUsername,
                    request.ItemVariantId);

                return Json(new
                {
                    success = true,
                    message = "Stock updated successfully.",
                    data = (object?)null
                });
            }
            catch (SqlException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = (object?)null
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStock(int stockId)
        {
            try
            {
                var deleted = await _itemStockRepository.DeleteAsync(stockId);

                return Json(new
                {
                    success = deleted,
                    message = deleted ? "Stock deleted successfully." : "Stock not found.",
                    data = (object?)null
                });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = (object?)null });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalStockPerItem()
        {
            try
            {
                var data = await _inventoryService.GetTotalStockPerItemAsync();

                var model = data.Select(x => new ItemStockViewModel
                {
                    ItemID = x.ItemID,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    BaseUnit = x.BaseUnit,
                    Quantity = x.Quantity
                }).ToList();

                return PartialView("Partials/_InventoryOverviewPartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}