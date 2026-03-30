using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemTypeController : Controller
    {
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemTypeController(IItemTypeRepository itemTypeRepository)
        {
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var itemTypes = await _itemTypeRepository.GetAllAsync();
            return View(itemTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ItemTypeViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var itemType = new ItemType
            {
                ItemTypeName = viewModel.ItemTypeName,
                Sort = viewModel.Sort,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };
            await _itemTypeRepository.AddAsync(itemType);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var itemType = await _itemTypeRepository.GetByIdAsync(id);
            if (itemType == null) return NotFound();

            var viewModel = new ItemTypeViewModel
            {
                ItemTypeID = itemType.ItemTypeID,
                ItemTypeName = itemType.ItemTypeName,
                Sort = itemType.Sort
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var itemType = new ItemType
            {
                ItemTypeID = viewModel.ItemTypeID,
                ItemTypeName = viewModel.ItemTypeName,
                Sort = viewModel.Sort,
                UpdatedBy = "Admin",
                UpdatedDate = DateTime.Now
            };
            await _itemTypeRepository.UpdateAsync(itemType);
            return RedirectToAction(nameof(Index));
        }
    }
}
