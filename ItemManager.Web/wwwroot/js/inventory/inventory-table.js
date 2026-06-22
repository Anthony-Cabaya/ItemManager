window.InventoryTable = {

    loadByItem: function (itemId) {
        fetch(`/Inventory/GetByItem?itemId=${itemId}`)
            .then(r => r.text())
            .then(html => {
                document.getElementById('stock-table-container').innerHTML = html;
                this.initRows();
            });
    },

    loadByLocation: function (locationId) {
        fetch(`/Inventory/GetByLocation?locationId=${locationId}`)
            .then(r => r.text())
            .then(html => {
                document.getElementById('stock-location-container').innerHTML = html;
                this.initRows();
            });
    },

    initRows: function () {
        const rows = document.querySelectorAll(".stock-row");

        rows.forEach(row => {
            row.addEventListener("click", function () {

                InventoryState.selectedStockId = this.dataset.id;
                InventoryState.selectedItemId = parseInt(this.dataset.itemId);
                InventoryState.selectedLocationId = parseInt(this.dataset.locationId);
                InventoryState.selectedItemName = this.dataset.itemName;
                InventoryState.selectedItemCode = this.dataset.itemCode;
                InventoryState.selectedLocationName = this.dataset.locationName;
                InventoryState.currentQuantity = parseFloat(this.dataset.quantity);
                InventoryState.currentMinStock = parseFloat(this.dataset.minStock || 0);
                InventoryState.selectedVariantId =
                    this.dataset.variantId
                        ? parseInt(this.dataset.variantId)
                        : null;

                InventoryToolbar.update();
            });
        });
    }

};