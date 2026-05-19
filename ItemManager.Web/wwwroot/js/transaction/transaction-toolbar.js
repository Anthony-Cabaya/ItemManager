window.TransactionToolbar = {

    switchTab(tab) {

        TransactionState.activeTab = tab;

        document
            .querySelectorAll('#transactionTabs .nav-link')
            .forEach(link => link.classList.remove('active'));

        const activeTab = document.getElementById(`tab-${tab}`);

        if (activeTab) {
            activeTab.classList.add('active');
        }

        document.getElementById('pane-recent').style.display =
            tab === 'recent' ? '' : 'none';

        document.getElementById('pane-by-item').style.display =
            tab === 'by-item' ? '' : 'none';

        document.getElementById('pane-by-location').style.display =
            tab === 'by-location' ? '' : 'none';

        if (tab === 'recent') {
            TransactionTable.loadRecent();
        }
    }
};