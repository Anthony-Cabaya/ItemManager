window.TransactionModals = {

    showError(errorId, message) {

        const errorDiv = document.getElementById(errorId);

        if (!errorDiv) {
            return;
        }

        errorDiv.textContent = message;
        errorDiv.classList.remove('d-none');
    },

    hideError(errorId) {

        const errorDiv = document.getElementById(errorId);

        if (!errorDiv) {
            return;
        }

        errorDiv.classList.add('d-none');
        errorDiv.textContent = '';
    },

    hasSelection() {

        return TransactionState.selectedItemId
            && TransactionState.selectedLocationId;
    },

    openStockIn() {

        this.hideError('stock-in-error');

        if (!this.hasSelection()) {
            this.showError(
                'stock-in-error',
                'Please select a transaction row first.');
        }

        document.getElementById('stock-in-item-id').value =
            TransactionState.selectedItemId || '';

        document.getElementById('stock-in-location-id').value =
            TransactionState.selectedLocationId || '';

        document.getElementById('stock-in-item-display').textContent =
            `${TransactionState.selectedItemCode} - ${TransactionState.selectedItemName}`;

        document.getElementById('stock-in-location-display').textContent =
            TransactionState.selectedLocationName;

        document.getElementById('stock-in-quantity').value = '';
        document.getElementById('stock-in-note').value = '';

        openModal('stockInModal');
    },

    openStockOut() {

        this.hideError('stock-out-error');

        if (!this.hasSelection()) {
            this.showError(
                'stock-out-error',
                'Please select a transaction row first.');
        }

        document.getElementById('stock-out-item-id').value =
            TransactionState.selectedItemId || '';

        document.getElementById('stock-out-location-id').value =
            TransactionState.selectedLocationId || '';

        document.getElementById('stock-out-item-display').textContent =
            `${TransactionState.selectedItemCode} - ${TransactionState.selectedItemName}`;

        document.getElementById('stock-out-location-display').textContent =
            TransactionState.selectedLocationName;

        document.getElementById('stock-out-available-display').textContent =
            TransactionState.selectedAvailableQty;

        document.getElementById('stock-out-quantity').value = '';
        document.getElementById('stock-out-note').value = '';

        openModal('stockOutModal');
    },

    openHold() {

        this.hideError('hold-error');

        if (!this.hasSelection()) {
            this.showError(
                'hold-error',
                'Please select a transaction row first.');
        }

        document.getElementById('hold-item-id').value =
            TransactionState.selectedItemId || '';

        document.getElementById('hold-location-id').value =
            TransactionState.selectedLocationId || '';

        document.getElementById('hold-item-display').textContent =
            `${TransactionState.selectedItemCode} - ${TransactionState.selectedItemName}`;

        document.getElementById('hold-location-display').textContent =
            TransactionState.selectedLocationName;

        document.getElementById('hold-available-display').textContent =
            TransactionState.selectedAvailableQty;

        document.getElementById('hold-quantity').value = '';
        document.getElementById('hold-note').value = '';

        openModal('holdModal');
    },

    openReleaseHold() {

        this.hideError('release-hold-error');

        if (!this.hasSelection()) {
            this.showError(
                'release-hold-error',
                'Please select a transaction row first.');
        }

        document.getElementById('release-hold-item-id').value =
            TransactionState.selectedItemId || '';

        document.getElementById('release-hold-location-id').value =
            TransactionState.selectedLocationId || '';

        document.getElementById('release-hold-item-display').textContent =
            `${TransactionState.selectedItemCode} - ${TransactionState.selectedItemName}`;

        document.getElementById('release-hold-location-display').textContent =
            TransactionState.selectedLocationName;

        document.getElementById('release-hold-reserved-display').textContent =
            TransactionState.selectedReservedQty;

        document.getElementById('release-hold-quantity').value = '';
        document.getElementById('release-hold-note').value = '';

        openModal('releaseHoldModal');
    },

    async submitStockIn() {

        const quantity = parseFloat(
            document.getElementById('stock-in-quantity').value);

        if (!quantity || quantity <= 0) {

            this.showError(
                'stock-in-error',
                'Quantity must be greater than zero.');

            return;
        }

        const payload = {
            itemId: document.getElementById('stock-in-item-id').value,
            locationId: document.getElementById('stock-in-location-id').value,
            quantity: quantity,
            referenceNote: document.getElementById('stock-in-note').value
        };

        const response = await postJson('/Transaction/StockIn', payload);

        if (response.success) {

            closeModal('stockInModal');

            showToast(response.message, 'create');

            TransactionTable.reloadCurrentTab();

            return;
        }

        this.showError('stock-in-error', response.message);
    },

    async submitStockOut() {

        const quantity = parseFloat(
            document.getElementById('stock-out-quantity').value);

        if (!quantity || quantity <= 0) {

            this.showError(
                'stock-out-error',
                'Quantity must be greater than zero.');

            return;
        }

        const payload = {
            itemId: document.getElementById('stock-out-item-id').value,
            locationId: document.getElementById('stock-out-location-id').value,
            quantity: quantity,
            referenceNote: document.getElementById('stock-out-note').value
        };

        const response = await postJson('/Transaction/StockOut', payload);

        if (response.success) {

            closeModal('stockOutModal');

            showToast(response.message, 'edit');

            TransactionTable.reloadCurrentTab();

            return;
        }

        this.showError('stock-out-error', response.message);
    },

    async submitHold() {

        const quantity = parseFloat(
            document.getElementById('hold-quantity').value);

        if (!quantity || quantity <= 0) {

            this.showError(
                'hold-error',
                'Quantity must be greater than zero.');

            return;
        }

        const payload = {
            itemId: document.getElementById('hold-item-id').value,
            locationId: document.getElementById('hold-location-id').value,
            quantity: quantity,
            referenceNote: document.getElementById('hold-note').value
        };

        const response = await postJson('/Transaction/Hold', payload);

        if (response.success) {

            closeModal('holdModal');

            showToast(response.message, 'edit');

            TransactionTable.reloadCurrentTab();

            return;
        }

        this.showError('hold-error', response.message);
    },

    async submitReleaseHold() {

        const quantity = parseFloat(
            document.getElementById('release-hold-quantity').value);

        if (!quantity || quantity <= 0) {

            this.showError(
                'release-hold-error',
                'Quantity must be greater than zero.');

            return;
        }

        const payload = {
            itemId: document.getElementById('release-hold-item-id').value,
            locationId: document.getElementById('release-hold-location-id').value,
            quantity: quantity,
            referenceNote: document.getElementById('release-hold-note').value
        };

        const response = await postJson('/Transaction/ReleaseHold', payload);

        if (response.success) {

            closeModal('releaseHoldModal');

            showToast(response.message, 'edit');

            TransactionTable.reloadCurrentTab();

            return;
        }

        this.showError('release-hold-error', response.message);
    }
};