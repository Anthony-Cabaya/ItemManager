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

        public class AddVariantRequest
        {
            public int ItemID { get; set; }
            public string VariantCode { get; set; } = "";
            public string VariantName { get; set; } = "";
            public string? AttributesText { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class SaveAttributesRequest
        {
            public int ItemID { get; set; }
            public List<AttributeDto> Attributes { get; set; } = new();
        }

        public class AttributeDto
        {
            public string AttributeName { get; set; } = "";
            public List<string> Values { get; set; } = new();
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
                ReservedQuantity = v.ReservedQuantity,
                AttributesText = v.AttributesText,
                AttributeValues = v.AttributeValues
                    .Select(av => new ItemAttributeValueViewModel
                    {
                        ItemAttributeValueID = av.ItemAttributeValueID,
                        ItemAttributeID = av.ItemAttributeID,
                        ValueLabel = av.ValueLabel,
                        Abbreviation = av.Abbreviation,
                        AttributeName = av.AttributeName
                    }).ToList()
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

            return View("Variants", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByItem(int itemId)
        {
            var variants = await _variantRepo.GetByItemAsync(itemId);
            var model = variants.Select(Map).ToList();

            return PartialView("Partials/_VariantTablePartial", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetNextVariantCode(int itemId)
        {
            var item = await _itemRepo.GetByIdAsync(itemId);

            if (item == null)
                return Json(new { success = false, message = "Item not found" });

            int attempt = 1;
            string code;

            do
            {
                code = $"{item.ItemCode}-V{attempt:D3}";
                attempt++;
            }
            while (await _variantRepo.ExistsAsync(itemId, code) && attempt <= 999);

            return Json(new
            {
                success = true,
                data = new { code }
            });
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<JsonResult> SaveAttributes([FromBody] SaveAttributesRequest request)
        {
            var existingVariants = await _variantRepo.GetByItemAsync(request.ItemID);

            if (!existingVariants.Any())
                await _attributeRepo.DeleteByItemAsync(request.ItemID);

            var result = new List<object>();

            foreach (var attr in request.Attributes)
            {
                var attribute = new ItemAttribute
                {
                    ItemID = request.ItemID,
                    AttributeName = attr.AttributeName,
                    Sort = 0,
                    Values = attr.Values.Select((v, i) =>
                        new ItemAttributeValue
                        {
                            ValueLabel = v,
                            Abbreviation = v.ToUpper().Replace(" ", ""),
                            Sort = i
                        }).ToList()
                };

                var attrId = await _attributeRepo.AddAsync(attribute, CurrentUsername);
                var saved = await _attributeRepo.GetByIdAsync(attrId);

                if (saved != null)
                {
                    result.Add(new
                    {
                        attributeId = saved.ItemAttributeID,
                        attributeName = saved.AttributeName,
                        values = saved.Values.Select(v => new
                        {
                            id = v.ItemAttributeValueID,
                            label = v.ValueLabel
                        })
                    });
                }
            }

            return JsonSuccess("Attributes saved.", result);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<JsonResult> BulkSave([FromBody] BulkSaveVariantsViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonFail("Invalid request.");

            var checkedRows = model.Rows?
                .Where(x => x.IsChecked)
                .DistinctBy(x => x.VariantCode)
                .ToList() ?? new();

            int saved = 0;

            foreach (var row in checkedRows)
            {
                var exists = await _variantRepo
                    .ExistsAsync(model.ItemID, row.VariantCode);

                if (exists) continue;

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

            return JsonSuccess(
                $"{saved} variant(s) saved.",
                new { saved }
            );
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
                AttributesText = model.AttributesText?.Trim(),
                IsActive = model.IsActive,
                UpdatedBy = CurrentUsername,
                UpdatedDate = DateTime.Now
            };

            await _variantRepo.UpdateAsync(variant, CurrentUsername);

            return JsonSuccess("Variant updated successfully.");
        }

        [HttpPost]
        public async Task<JsonResult> AddSingle([FromBody] AddVariantRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.VariantCode))
            {
                return JsonFail("Variant Code is required.");
            }

            if (string.IsNullOrWhiteSpace(request.VariantName))
            {
                return JsonFail("Variant Name is required.");
            }

            var exists = await _variantRepo.ExistsAsync(
                    request.ItemID,
                    request.VariantCode);

            if (exists)
            {
                return JsonFail("Variant Code already exists.");
            }

            var variant = new ItemVariant
            {
                ItemID = request.ItemID,
                VariantCode = request.VariantCode.Trim().ToUpper(),
                VariantName = request.VariantName.Trim(),
                AttributesText = request.AttributesText?.Trim(),
                IsActive = request.IsActive,
                Sort = 0,
                CreatedBy = CurrentUsername,
                CreatedDate = DateTime.Now,

                AttributeValues = new()
            };

            await _variantRepo.AddAsync(variant, CurrentUsername);

            return JsonSuccess("Variant added successfully.");
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