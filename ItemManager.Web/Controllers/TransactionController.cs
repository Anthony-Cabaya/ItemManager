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
        private readonly IItemRepository _itemRepository;

        public TransactionController(
            ITransactionService transactionService,
            ILocationRepository locationRepository,
            IItemRepository itemRepository)
        {
            _transactionService = transactionService;
            _locationRepository = locationRepository;
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? itemId,
            int? locationId)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(TransactionLogViewModel model)
        {
            try
            {
                if (model.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Quantity must be greater than zero."
                    });
                }

                await _transactionService.StockInAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername);

                return Json(new
                {
                    success = true,
                    message = "Stock in recorded."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOut(TransactionLogViewModel model)
        {
            try
            {
                if (model.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Quantity must be greater than zero."
                    });
                }

                await _transactionService.StockOutAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername);

                return Json(new
                {
                    success = true,
                    message = "Stock out recorded."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hold(TransactionLogViewModel model)
        {
            try
            {
                if (model.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Quantity must be greater than zero."
                    });
                }

                await _transactionService.HoldAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername);

                return Json(new
                {
                    success = true,
                    message = "Stock hold recorded."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReleaseHold(TransactionLogViewModel model)
        {
            try
            {
                if (model.Quantity <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Quantity must be greater than zero."
                    });
                }

                await _transactionService.ReleaseHoldAsync(
                    model.ItemID,
                    model.LocationID,
                    model.Quantity,
                    model.ReferenceNote,
                    CurrentUsername);

                return Json(new
                {
                    success = true,
                    message = "Stock hold released."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}