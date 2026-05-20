using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public class DeleteLocationRequest
        {
            public List<int> Ids { get; set; } = new();
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            try
            {
                var result = await _locationRepository.GetPagedAsync(pageNumber, pageSize, search);

                var viewModel = new Core.Helpers.PagedResult<LocationViewModel>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    Items = result.Items.Select(x => new LocationViewModel
                    {
                        LocationID = x.LocationID,
                        LocationName = x.LocationName,
                        IsActive = x.IsActive,
                        Sort = x.Sort,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate
                    }).ToList()
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
        public async Task<IActionResult> Create([FromBody] LocationViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed." });

            try
            {
                var entity = new Location
                {
                    LocationName = model.LocationName,
                    IsActive = model.IsActive,
                    Sort = model.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _locationRepository.AddAsync(entity);

                return Json(new { success = true, message = "Location created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] LocationViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed." });

            try
            {
                var entity = new Location
                {
                    LocationID = model.LocationID,
                    LocationName = model.LocationName,
                    IsActive = model.IsActive,
                    Sort = model.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };

                await _locationRepository.UpdateAsync(entity);

                return Json(new { success = true, message = "Location updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteLocationRequest request)
        {
            try
            {
                if (request.Ids == null || !request.Ids.Any())
                    return Json(new { success = false, message = "No locations selected." });

                foreach (var id in request.Ids)
                {
                    var hasStock = await _locationRepository.HasStockAsync(id);

                    if (hasStock)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Cannot delete location with stock records."
                        });
                    }

                    await _locationRepository.DeleteAsync(id);
                }

                return Json(new
                {
                    success = true,
                    message = $"{request.Ids.Count} location(s) deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}