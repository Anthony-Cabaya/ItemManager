window.InventoryToolbar = {

    switchTab: function (tab) {
        InventoryState.activeTab = tab;

        document.querySelectorAll('#inventoryTabs .nav-link')
            .forEach(b => b.classList.remove('active'));

        document.getElementById(`tab-${tab}`)
            .classList.add('active');

        document.getElementById('pane-by-item').style.display =
            tab === 'by-item' ? '' : 'none';

        document.getElementById('pane-by-location').style.display =
            tab === 'by-location' ? '' : 'none';
    },

    update: function () {
        const hasSelection = !!InventoryState.selectedStockId;

        const editStock = document.getElementById('btn-edit-stock');
        const deleteStock = document.getElementById('btn-delete-stock');
        const editStockLoc = document.getElementById('btn-edit-stock-loc');
        const deleteStockLoc = document.getElementById('btn-delete-stock-loc');

        if (editStock) editStock.disabled = !hasSelection;
        if (deleteStock) deleteStock.disabled = !hasSelection;
        if (editStockLoc) editStockLoc.disabled = !hasSelection;
        if (deleteStockLoc) deleteStockLoc.disabled = !hasSelection;
    }
};