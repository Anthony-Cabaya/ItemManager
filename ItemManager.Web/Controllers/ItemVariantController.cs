using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ItemManager.Web.Controllers
{
    public class ItemVariantController : BaseController
    {
        private readonly IItemVariantRepository _variantRepo;
        private readonly IItemAttributeRepository _attributeRepo;
        private readonly IVariantCodeService _codeService;
        private readonly IItemRepository _itemRepo;

        public ItemVariantController(
            IItemVariantRepository variantRepo,
            IItemAttributeRepository attributeRepo,
            IVariantCodeService codeService,
            IItemRepository itemRepo)
        {
            _variantRepo = variantRepo;
            _attributeRepo = attributeRepo;
            _codeService = codeService;
            _itemRepo = itemRepo;
        }

        public class SetActiveVariantRequest
        {
            public int VariantId { get; set; }
            public bool IsActive { get; set; }
        }

        public class DeleteVariantRequest
        {
            public int VariantId { get; set; }
        }

        public class SuggestCodeRequest
        {
            public string ParentCode { get; set; } = string.Empty;
            public List<string> Abbreviations { get; set; } = new();
        }

        private JsonResult JsonSuccess(string message, object? data = null)
            => Json(new { success = true, message, data });

        private JsonResult JsonFail(string message)
            => Json(new { success = false, message, data = (object?)null });

        private ItemVariantViewModel Map(ItemVariant v)
        {
            return new ItemVariantViewModel
            {
                ItemVariantID = v.ItemVariantID,
                ItemID = v.ItemID,
                VariantCode = v.VariantCode,
                VariantName = v.VariantName,
                IsActive = v.IsActive,
                Sort = v.Sort,
                ItemName = v.ItemName,
                ItemCode = v.ItemCode,
                Quantity = v.Quantity,
                ReservedQuantity = v.ReservedQuantity
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(int itemId)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);
            if (item == null)
                return NotFound();

            var variants = await _variantRepo.GetByItemAsync(itemId);
            var attributes = await _attributeRepo.GetByItemAsync(itemId);

            var model = variants.Select(Map).ToList();

            ViewBag.Item = item;
            ViewBag.Attributes = attributes;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByItem(int itemId)
        {
            var variants = await _variantRepo.GetByItemAsync(itemId);
            var model = variants.Select(Map).ToList();

            return PartialView("Partials/_VariantTablePartial", model);
        }

        [HttpPost]
        public async Task<JsonResult> BulkSave([FromBody] BulkSaveVariantsViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonFail("Invalid request.");

            var checkedRows = model.Rows.Where(x => x.IsChecked).ToList();
            int saved = 0;

            foreach (var row in checkedRows)
            {
                var exists = await _variantRepo.ExistsAsync(model.ItemID, row.VariantCode);
                if (exists)
                    continue;

                var variant = new ItemVariant
                {
                    ItemID = model.ItemID,
                    VariantCode = row.VariantCode,
                    VariantName = row.VariantName,
                    IsActive = true,
                    Sort = 0,
                    CreatedBy = CurrentUsername,
                    CreatedDate = DateTime.Now,
                    AttributeValues = row.AttributeValueIds
                        .Select(id => new ItemAttributeValue
                        {
                            ItemAttributeValueID = id
                        }).ToList()
                };

                await _variantRepo.AddAsync(variant, CurrentUsername);
                saved++;
            }

            return JsonSuccess($"{saved} variant(s) saved.", new { saved });
        }

        [HttpPost]
        public async Task<JsonResult> SetActive([FromBody] SetActiveVariantRequest request)
        {
            await _variantRepo.SetActiveAsync(request.VariantId, request.IsActive, CurrentUsername);
            return JsonSuccess("Updated successfully.");
        }

        [HttpPost]
        public async Task<JsonResult> Edit([FromBody] ItemVariantViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonFail("Validation failed.");

            var exists = await _variantRepo.ExistsAsync(
                    model.ItemID,
                    model.VariantCode,
                    model.ItemVariantID
                );

            if (exists)
                return JsonFail("Variant code already exists.");

            var variant = new ItemVariant
            {
                ItemVariantID = model.ItemVariantID,
                ItemID = model.ItemID,
                VariantCode = model.VariantCode,
                VariantName = model.VariantName,
                IsActive = model.IsActive,
                Sort = model.Sort,
                UpdatedBy = CurrentUsername,
                UpdatedDate = DateTime.Now
            };

            await _variantRepo.UpdateAsync(variant, CurrentUsername);

            return JsonSuccess("Variant updated successfully.");
        }

        [HttpPost]
        public async Task<JsonResult> Delete([FromBody] DeleteVariantRequest request)
        {
            var hasStock = await _variantRepo.HasStockAsync(request.VariantId);

            if (hasStock)
                return JsonFail("Cannot delete variant with existing stock records.");

            await _variantRepo.DeleteAsync(request.VariantId);

            return JsonSuccess("Deleted successfully.");
        }

        [HttpPost]
        public JsonResult SuggestCode([FromBody] SuggestCodeRequest request)
        {
            var code = _codeService.SuggestCode(request.ParentCode, request.Abbreviations);
            return JsonSuccess("Generated successfully.", new { code });
        }
    }
}