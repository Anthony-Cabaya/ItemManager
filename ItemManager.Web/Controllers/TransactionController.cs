using ItemManager.Core.Interfaces;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.Controllers
{
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;
        private readonly ILocationRepository _locationRepository;
        private readonly IItemRepository _itemRepo;
        private readonly IItemVariantRepository _variantRepo;

        public TransactionController(
            ITransactionService transactionService,
            ILocationRepository locationRepository,
            IItemRepository itemRepo,
            IItemVariantRepository variantRepo)
        {
            _transactionService = transactionService;
            _locationRepository = locationRepository;
            _itemRepo = itemRepo;
            _variantRepo = variantRepo;
        }

        private JsonResult JsonSuccess(string message)
            => Json(new { success = true, message });

        private JsonResult JsonFail(string message)
            => Json(new { success = false, message });

        private class TransactionValidationResult
        {
            public bool IsValid { get; set; }
            public string? ErrorMessage { get; set; }

            public static TransactionValidationResult Success()
                => new() { IsValid = true };

            public static TransactionValidationResult Fail(string message)
                => new() { IsValid = false, ErrorMessage = message };
        }

        private async Task<TransactionValidationResult> ValidateAsync(TransactionLogViewModel model)
        {
            if (model.Quantity <= 0)
                return TransactionValidationResult.Fail("Quantity must be greater than zero.");

            var item = await _itemRepo.GetByIdAsync(model.ItemID);

            if (item != null &&
                item.VariantCount > 0 &&
                model.ItemVariantID == null)
            {
                return TransactionValidationResult.Fail("Please select a variant for this item.");
            }

            return TransactionValidationResult.Success();
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? itemId, int? locationId)
        {
            var locations = await _locationRepository.GetAllAsync();

            ViewBag.Locations = locations
                .Select(x => new SelectListItem
                {
                    Value = x.LocationID.ToString(),
                    Text = x.LocationName
                })
                .ToList();

            ViewBag.ItemId = itemId;
            ViewBag.LocationId = locationId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetByItem(int itemId)
        {
            var result = await _transactionService.GetByItemAsync(itemId);

            var model = result.Select(x => new TransactionLogViewModel
            {
                TransactionID = x.TransactionID,
                ItemID = x.ItemID,
                LocationID = x.LocationID,
                ItemVariantID = x.ItemVariantID,
                VariantName = x.VariantName,
                TransactionType = x.TransactionType,
                Quantity = x.Quantity,
                ReferenceNote = x.ReferenceNote,
                TransactionDate = x.TransactionDate,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ItemName = x.ItemName,
                ItemCode = x.ItemCode,
                LocationName = x.LocationName
            }).ToList();

            return PartialView("Partials/_TransactionTablePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            var result = await _transactionService.GetByLocationAsync(locationId);

            var model = result.Select(x => new TransactionLogViewModel
            {
                TransactionID = x.TransactionID,
                ItemID = x.ItemID,
                LocationID = x.LocationID,
                ItemVariantID = x.ItemVariantID,
                VariantName = x.VariantName,
                TransactionType = x.TransactionType,
                Quantity = x.Quantity,
                ReferenceNote = x.ReferenceNote,
                TransactionDate = x.TransactionDate,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ItemName = x.ItemName,
                ItemCode = x.ItemCode,
                LocationName = x.LocationName
            }).ToList();

            return PartialView("Partials/_TransactionTablePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var result = await _transactionService.GetRecentAsync(50);

            var model = result.Select(x => new TransactionLogViewModel
            {
                TransactionID = x.TransactionID,
                ItemID = x.ItemID,
                LocationID = x.LocationID,
                ItemVariantID = x.ItemVariantID,
                VariantName = x.VariantName,
                TransactionType = x.TransactionType,
                Quantity = x.Quantity,
                ReferenceNote = x.ReferenceNote,
                TransactionDate = x.TransactionDate,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ItemName = x.ItemName,
                ItemCode = x.ItemCode,
                LocationName = x.LocationName
            }).ToList();

            return PartialView("Partials/_TransactionTablePartial", model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> StockIn([FromBody] TransactionLogViewModel model)
        {
            var validation = await ValidateAsync(model);
            if (!validation.IsValid)
                return JsonFail(validation.ErrorMessage!);

            try
            {
                await _transactionService.StockInAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername,
                    model.ItemVariantID);

                return JsonSuccess("Stock in recorded.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> StockOut([FromBody] TransactionLogViewModel model)
        {
            var validation = await ValidateAsync(model);
            if (!validation.IsValid)
                return JsonFail(validation.ErrorMessage!);

            try
            {
                await _transactionService.StockOutAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername,
                    model.ItemVariantID);

                return JsonSuccess("Stock out recorded.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Hold([FromBody] TransactionLogViewModel model)
        {
            var validation = await ValidateAsync(model);
            if (!validation.IsValid)
                return JsonFail(validation.ErrorMessage!);

            try
            {
                await _transactionService.HoldAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername,
                    model.ItemVariantID);

                return JsonSuccess("Stock hold recorded.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ReleaseHold([FromBody] TransactionLogViewModel model)
        {
            var validation = await ValidateAsync(model);
            if (!validation.IsValid)
                return JsonFail(validation.ErrorMessage!);

            try
            {
                await _transactionService.ReleaseHoldAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername,
                    model.ItemVariantID);

                return JsonSuccess("Stock hold released.");
            }
            catch (Exception ex)
            {
                return JsonFail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVariantsByItem(int itemId)
        {
            var variants = await _variantRepo.GetByItemAsync(itemId);

            var result = variants
                .Where(x => x.IsActive)
                .Select(x => new
                {
                    itemVariantId = x.ItemVariantID,
                    variantCode = x.VariantCode,
                    variantName = x.VariantName
                });

            return Json(result);
        }
    }
}