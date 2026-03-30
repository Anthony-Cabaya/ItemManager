using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemController(IItemRepository itemRepository, IItemTypeRepository itemTypeRepository)
        {
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _itemRepository.GetAllAsync();
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ItemViewModel
            {
                ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList();
                return View(viewModel);
            }

            var item = new Item
            {
                ItemName = viewModel.ItemName,
                ItemTypeID = viewModel.ItemTypeID,
                Sort = viewModel.Sort,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };
            await _itemRepository.AddAsync(item);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            var viewModel = new ItemViewModel
            {
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                ItemTypeID = item.ItemTypeID,
                Sort = item.Sort,
                ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ItemTypes = (await _itemTypeRepository.GetAllAsync()).ToList();
                return View(viewModel);
            }

            var item = new Item
            {
                ItemID = viewModel.ItemID,
                ItemName = viewModel.ItemName,
                ItemTypeID = viewModel.ItemTypeID,
                Sort = viewModel.Sort,
                UpdatedBy = "Admin",
                UpdatedDate = DateTime.Now
            };
            await _itemRepository.UpdateAsync(item);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _itemRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
