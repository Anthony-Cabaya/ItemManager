window.addEventListener('DOMContentLoaded', () => {

    TransactionToolbar.switchTab('recent');

    document
        .getElementById('tab-recent')
        .addEventListener('click', () => {
            TransactionToolbar.switchTab('recent');
        });

    document
        .getElementById('tab-by-item')
        .addEventListener('click', () => {
            TransactionToolbar.switchTab('by-item');
        });

    document
        .getElementById('tab-by-location')
        .addEventListener('click', () => {
            TransactionToolbar.switchTab('by-location');
        });

    document
        .getElementById('btn-stock-in')
        .addEventListener('click', TransactionModals.openStockIn.bind(TransactionModals));

    document
        .getElementById('btn-stock-out')
        .addEventListener('click', TransactionModals.openStockOut.bind(TransactionModals));

    document
        .getElementById('btn-hold')
        .addEventListener('click', TransactionModals.openHold.bind(TransactionModals));

    document
        .getElementById('btn-release-hold')
        .addEventListener('click', TransactionModals.openReleaseHold.bind(TransactionModals));

    const itemSearchInput =
        document.getElementById('item-search-input');

    const itemSearchClear =
        document.getElementById('item-search-clear');

    document
        .getElementById('btn-search-item')
        .addEventListener('click', () => {

            const value = itemSearchInput.value.trim();

            if (!value) {
                return;
            }

            TransactionState.currentItemId = value;

            TransactionTable.loadByItem(value);
        });

    itemSearchInput.addEventListener('keydown', e => {

        if (e.key !== 'Enter') {
            return;
        }

        document
            .getElementById('btn-search-item')
            .click();
    });

    itemSearchInput.addEventListener('input', () => {

        itemSearchClear.style.display =
            itemSearchInput.value.trim()
                ? 'block'
                : 'none';
    });

    itemSearchClear.addEventListener('click', () => {

        itemSearchInput.value = '';

        itemSearchClear.style.display = 'none';

        document.getElementById('item-table-container').innerHTML = `
            <div class="text-center text-muted py-4">
                Search for an item to view transactions.
            </div>`;
    });

    document
        .getElementById('btn-stock-in-item')
        .addEventListener('click', TransactionModals.openStockIn.bind(TransactionModals));

    document
        .getElementById('btn-stock-out-item')
        .addEventListener('click', TransactionModals.openStockOut.bind(TransactionModals));

    document
        .getElementById('btn-hold-item')
        .addEventListener('click', TransactionModals.openHold.bind(TransactionModals));

    document
        .getElementById('btn-release-hold-item')
        .addEventListener('click', TransactionModals.openReleaseHold.bind(TransactionModals));

    document
        .getElementById('location-filter-select')
        .addEventListener('change', e => {

            const value = e.target.value;

            TransactionState.currentLocationId = value;

            if (!value) {
                return;
            }

            TransactionTable.loadByLocation(value);
        });

    document
        .getElementById('btn-stock-in-loc')
        .addEventListener('click', TransactionModals.openStockIn.bind(TransactionModals));

    document
        .getElementById('btn-stock-out-loc')
        .addEventListener('click', TransactionModals.openStockOut.bind(TransactionModals));

    document
        .getElementById('btn-hold-loc')
        .addEventListener('click', TransactionModals.openHold.bind(TransactionModals));

    document
        .getElementById('btn-release-hold-loc')
        .addEventListener('click', TransactionModals.openReleaseHold.bind(TransactionModals));

    document
        .getElementById('btn-confirm-stock-in')
        .addEventListener('click', TransactionModals.submitStockIn.bind(TransactionModals));

    document
        .getElementById('btn-confirm-stock-out')
        .addEventListener('click', TransactionModals.submitStockOut.bind(TransactionModals));

    document
        .getElementById('btn-confirm-hold')
        .addEventListener('click', TransactionModals.submitHold.bind(TransactionModals));

    document
        .getElementById('btn-confirm-release-hold')
        .addEventListener('click', TransactionModals.submitReleaseHold.bind(TransactionModals));
});