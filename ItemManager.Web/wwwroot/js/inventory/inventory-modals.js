window.InventoryModals = {

    openSetStock: function () {

        document.getElementById("set-stock-id").value = InventoryState.selectedStockId || "";
        document.getElementById("set-stock-item-id").value = InventoryState.selectedItemId;
        document.getElementById("set-stock-location-id").value = InventoryState.selectedLocationId;

        document.getElementById("set-stock-item-display").innerText =
            InventoryState.selectedItemCode + " - " + InventoryState.selectedItemName;

        document.getElementById("set-stock-location-display").innerText =
            InventoryState.selectedLocationName;

        document.getElementById("set-stock-quantity").value = InventoryState.currentQuantity;
        document.getElementById("set-stock-minstock").value = InventoryState.currentMinStock;

        document.getElementById("set-stock-error").classList.add("d-none");

        openModal("setStockModal");
    },

    openDeleteStock: function () {

        document.getElementById("delete-stock-id").value = InventoryState.selectedStockId;

        document.getElementById("delete-stock-item-name").innerText = InventoryState.selectedItemName;
        document.getElementById("delete-stock-location-name").innerText = InventoryState.selectedLocationName;

        document.getElementById("delete-stock-error").classList.add("d-none");

        openModal("deleteStockModal");
    },

    submitSetStock: function () {

        const qty = parseFloat(document.getElementById("set-stock-quantity").value);

        if (qty < 0) {
            const err = document.getElementById("set-stock-error");
            err.innerText = "Quantity cannot be negative";
            err.classList.remove("d-none");
            return;
        }

        postJson("/Inventory/SetStock", {
            itemId: InventoryState.selectedItemId,
            locationId: InventoryState.selectedLocationId,
            quantity: qty,
            minStock: document.getElementById("set-stock-minstock").value
        }).then(res => {

            if (res.success) {
                closeModal("setStockModal");
                showToast(res.message, "edit");

                if (InventoryState.viewMode === "item") {
                    InventoryTable.loadByItem(InventoryState.selectedItemId);
                } else {
                    InventoryTable.loadByLocation(InventoryState.selectedLocationId);
                }
            } else {
                const err = document.getElementById("set-stock-error");
                err.innerText = res.message;
                err.classList.remove("d-none");
            }
        });
    },

    submitDeleteStock: function () {

        postJson("/Inventory/DeleteStock", {
            stockId: InventoryState.selectedStockId
        }).then(res => {

            if (res.success) {
                closeModal("deleteStockModal");
                showToast(res.message, "delete");

                InventoryState.selectedStockId = null;
                InventoryToolbar.update();

                if (InventoryState.viewMode === "item") {
                    InventoryTable.loadByItem(InventoryState.selectedItemId);
                } else {
                    InventoryTable.loadByLocation(InventoryState.selectedLocationId);
                }
            } else {
                const err = document.getElementById("delete-stock-error");
                err.innerText = res.message;
                err.classList.remove("d-none");
            }
        });
    }
};