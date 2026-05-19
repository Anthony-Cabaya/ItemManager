window.TransactionTable = {

    async loadRecent() {

        const container = document.getElementById('recent-table-container');

        container.innerHTML = `
            <div class="text-center text-muted py-4">
                Loading transactions...
            </div>`;

        const response = await fetch('/Transaction/GetRecent', {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const html = await response.text();

        container.innerHTML = html;

        this.initRows();
    },

    async loadByItem(itemId) {

        const container = document.getElementById('item-table-container');

        container.innerHTML = `
            <div class="text-center text-muted py-4">
                Loading transactions...
            </div>`;

        const response = await fetch(`/Transaction/GetByItem?itemId=${itemId}`, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const html = await response.text();

        container.innerHTML = html;

        this.initRows();
    },

    async loadByLocation(locationId) {

        const container = document.getElementById('location-table-container');

        container.innerHTML = `
            <div class="text-center text-muted py-4">
                Loading transactions...
            </div>`;

        const response = await fetch(`/Transaction/GetByLocation?locationId=${locationId}`, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const html = await response.text();

        container.innerHTML = html;

        this.initRows();
    },

    initRows() {

        const rows = document.querySelectorAll('.transaction-row');

        rows.forEach(row => {

            row.addEventListener('click', () => {

                document
                    .querySelectorAll('.transaction-row')
                    .forEach(r => r.classList.remove('table-active'));

                row.classList.add('table-active');

                TransactionState.selectedItemId =
                    row.dataset.itemId || null;

                TransactionState.selectedLocationId =
                    row.dataset.locationId || null;

                TransactionState.selectedItemName =
                    row.dataset.itemName || '';

                TransactionState.selectedItemCode =
                    row.dataset.itemCode || '';

                TransactionState.selectedLocationName =
                    row.dataset.locationName || '';

                TransactionState.selectedAvailableQty = 0;

                TransactionState.selectedReservedQty = 0;
            });

        });
    },

    reloadCurrentTab() {

        switch (TransactionState.activeTab) {

            case 'recent':
                this.loadRecent();
                break;

            case 'by-item':

                if (TransactionState.currentItemId) {
                    this.loadByItem(TransactionState.currentItemId);
                }

                break;

            case 'by-location':

                if (TransactionState.currentLocationId) {
                    this.loadByLocation(TransactionState.currentLocationId);
                }

                break;
        }
    }
};