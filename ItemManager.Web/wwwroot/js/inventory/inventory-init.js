window.addEventListener('DOMContentLoaded', function () {

    InventoryToolbar.switchTab('by-item');
    InventoryToolbar.update();

    document.getElementById('tab-by-item')
        ?.addEventListener('click', () => InventoryToolbar.switchTab('by-item'));

    document.getElementById('tab-by-location')
        ?.addEventListener('click', () => InventoryToolbar.switchTab('by-location'));

    const itemSearchInput = document.getElementById('item-search-input');
    const itemSearchClear = document.getElementById('item-search-clear');

    document.getElementById('btn-item-search')
        ?.addEventListener('click', function () {

            const itemId = itemSearchInput?.dataset.itemId;

            if (itemId) {
                InventoryTable.loadByItem(itemId);
            } else {
                document.getElementById('stock-table-container').innerHTML =
                    '<div class="text-muted text-center py-4">Please search for an item.</div>';
            }
        });

    itemSearchInput?.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            document.getElementById('btn-item-search')?.click();
        }
    });

    itemSearchInput?.addEventListener('input', function () {
        itemSearchClear.style.display = this.value ? 'block' : 'none';
    });

    itemSearchClear?.addEventListener('click', function () {
        itemSearchInput.value = '';
        itemSearchInput.removeAttribute('data-item-id');
        itemSearchClear.style.display = 'none';

        document.getElementById('stock-table-container').innerHTML =
            '<div class="text-muted text-center py-4">Search for an item to view stock.</div>';

        InventoryState.selectedStockId = null;
        InventoryToolbar.update();
    });

    document.getElementById('location-filter-select')
        ?.addEventListener('change', function () {
            if (this.value) {
                InventoryTable.loadByLocation(this.value);
            }
        });

    document.getElementById('btn-edit-stock')
        ?.addEventListener('click', InventoryModals.openSetStock);

    document.getElementById('btn-delete-stock')
        ?.addEventListener('click', InventoryModals.openDeleteStock);

    document.getElementById('btn-edit-stock-loc')
        ?.addEventListener('click', InventoryModals.openSetStock);

    document.getElementById('btn-delete-stock-loc')
        ?.addEventListener('click', InventoryModals.openDeleteStock);

    document.getElementById('btn-save-stock')
        ?.addEventListener('click', InventoryModals.submitSetStock);

    document.getElementById('btn-confirm-delete-stock')
        ?.addEventListener('click', InventoryModals.submitDeleteStock);
});