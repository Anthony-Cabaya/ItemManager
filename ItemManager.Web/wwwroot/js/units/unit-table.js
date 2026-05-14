window.UnitTable = {

    loadByCategory: function (categoryId) {

        const container = document.getElementById('unit-table-container');
        const url = `/Unit/GetUnitsByCategory?categoryId=${categoryId}`;

        fetch(url)
            .then(res => res.text())
            .then(html => {
                container.innerHTML = html;
                this.initRows();
            })
            .catch(err => {
                container.innerHTML =
                    `<div class="text-danger p-2">${err}</div>`;
            });
    },

    initRows: function () {

        const rows = document.querySelectorAll('.unit-row');

        rows.forEach(row => {
            row.addEventListener('click', () => {

                rows.forEach(r => r.classList.remove('table-active'));
                row.classList.add('table-active');

                UnitState.selectedUnitId = row.dataset.id;
                UnitState.selectedUnitName = row.dataset.name;
                UnitState.selectedUnitIsSystem = row.dataset.isSystem;

                UnitToolbar.update();
            });
        });
    },

    filterRows: function (search) {

        const rows = document.querySelectorAll('.unit-row');
        const clearBtn = document.getElementById('unit-search-clear');

        UnitState.currentSearch = search || '';

        const keyword = UnitState.currentSearch.toLowerCase();

        rows.forEach(row => {
            const nameCell = row.querySelector('td:nth-child(2)');
            const text = nameCell ? nameCell.textContent.toLowerCase() : '';

            row.style.display = text.includes(keyword) ? '' : 'none';
        });

        clearBtn.style.display = keyword.length > 0 ? 'block' : 'none';
    },

    clearSearch: function () {

        document.getElementById('unit-search').value = '';
        UnitState.currentSearch = '';

        const rows = document.querySelectorAll('.unit-row');
        rows.forEach(r => r.style.display = '');

        document.getElementById('unit-search-clear').style.display = 'none';
    }
};