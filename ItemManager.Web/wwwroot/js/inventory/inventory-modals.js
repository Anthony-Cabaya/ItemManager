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

        // Reset variant section before loading
        const variantWrapper = document.getElementById("set-stock-variant-wrapper");
        const variantSelect = document.getElementById("set-stock-variant-id");
        const variantError = document.getElementById("set-stock-variant-error");

        variantWrapper.style.display = "none";
        variantSelect.innerHTML = '<option value="">-- Select Variant --</option>';
        variantError.style.visibility = "hidden";
        variantError.textContent = "";

        // Load variants for the selected item
        const itemId = InventoryState.selectedItemId;
        if (itemId) {
            this._loadVariants(itemId, variantSelect, variantWrapper);
        }

        openModal("setStockModal");
    },

    _loadVariants: async function (itemId, selectEl, wrapperEl) {
        try {
            const response = await fetch(
                `/Transaction/GetVariantsByItem?itemId=${itemId}`
            );
            const data = await response.json();

            selectEl.innerHTML =
                '<option value="">-- Select Variant --</option>';

            if (!data || data.length === 0) {
                wrapperEl.style.display = "none";
                return;
            }

            data.forEach(v => {
                const option = document.createElement("option");
                option.value = v.itemVariantId;
                option.textContent = `${v.variantCode} — ${v.variantName}`;
                selectEl.appendChild(option);
            });

            wrapperEl.style.display = "block";

        } catch (err) {
            console.error("Variant load failed:", err);
            wrapperEl.style.display = "none";
        }
    },

    openDeleteStock: function () {

        document.getElementById("delete-stock-id").value = InventoryState.selectedStockId;

        document.getElementById("delete-stock-item-name").innerText = InventoryState.selectedItemName;
        document.getElementById("delete-stock-location-name").innerText = InventoryState.selectedLocationName;

        document.getElementById("delete-stock-error").classList.add("d-none");

        openModal("deleteStockModal");
    },

    submitSetStock: function () {

        const qty = parseFloat(
            document.getElementById("set-stock-quantity").value);

        if (qty < 0) {
            const err = document.getElementById("set-stock-error");
            err.innerText = "Quantity cannot be negative";
            err.classList.remove("d-none");
            return;
        }

        const variantWrapper = document.getElementById("set-stock-variant-wrapper");
        const variantSelect = document.getElementById("set-stock-variant-id");
        const variantError = document.getElementById("set-stock-variant-error");

        const wrapperVisible = variantWrapper.style.display === "block";
        const variantVal = parseInt(variantSelect.value);
        const itemVariantId = isNaN(variantVal) || variantVal === 0
            ? null
            : variantVal;

        if (wrapperVisible && !itemVariantId) {
            variantError.textContent = "Please select a variant.";
            variantError.style.visibility = "visible";
            return;
        }

        variantError.style.visibility = "hidden";
        variantError.textContent = "";

        postJson("/Inventory/SetStock", {
            itemId: InventoryState.selectedItemId,
            locationId: InventoryState.selectedLocationId,
            quantity: qty,
            minStock: document.getElementById("set-stock-minstock").value,
            itemVariantId: itemVariantId
        }).then(res => {

            if (res.success) {
                closeModal("setStockModal");
                showToast(res.message, "edit");

                if (InventoryState.viewMode === "item") {
                    InventoryTable.loadByItem(InventoryState.selectedItemId);
                } else {
                    InventoryTable.loadByLocation(
                        InventoryState.selectedLocationId);
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