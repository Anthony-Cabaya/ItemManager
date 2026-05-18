using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ItemManager.Web.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string search = "")
        {
            try
            {
                var result = await _locationRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    search);

                var viewModel = new Core.Helpers.PagedResult<LocationViewModel>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    Items = result.Items.Select(x => new LocationViewModel
                    {
                        LocationID = x.LocationID,
                        LocationName = x.LocationName,
                        Description = x.Description,
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
        public IActionResult Create()
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
                return BadRequest();

            return PartialView("_CreateLocationModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed." });

            try
            {
                var entity = new Location
                {
                    LocationName = model.LocationName,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    Sort = model.Sort,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now
                };

                await _locationRepository.AddAsync(entity);

                return Json(new { success = true, message = "Location created successfully." });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
                return BadRequest();

            try
            {
                var entity = await _locationRepository.GetByIdAsync(id);

                if (entity == null)
                    return NotFound();

                var model = new LocationViewModel
                {
                    LocationID = entity.LocationID,
                    LocationName = entity.LocationName,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                    Sort = entity.Sort,
                    CreatedBy = entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedDate = entity.UpdatedDate
                };

                return PartialView("_EditLocationModal", model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LocationViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed." });

            try
            {
                var entity = new Location
                {
                    LocationID = model.LocationID,
                    LocationName = model.LocationName,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    Sort = model.Sort,
                    UpdatedBy = CurrentUsername,
                    UpdatedDate = DateTime.Now
                };

                await _locationRepository.UpdateAsync(entity);

                return Json(new { success = true, message = "Location updated successfully." });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
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

                var deleted = await _locationRepository.DeleteAsync(id);

                return Json(new
                {
                    success = deleted,
                    message = deleted ? "Location deleted successfully." : "Location not found."
                });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}