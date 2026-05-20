window.LocationTable = {

    initRows: function () {

        const checkboxes = document.querySelectorAll('.row-checkbox');
        const selectAll = document.getElementById('selectAll');

        if (selectAll) {
            const newSelectAll = selectAll.cloneNode(true);
            selectAll.parentNode.replaceChild(newSelectAll, selectAll);
        }

        const freshSelectAll = document.getElementById('selectAll');
        const freshCheckboxes = document.querySelectorAll('.row-checkbox');

        freshCheckboxes.forEach(cb => {

            const newCb = cb.cloneNode(true);
            cb.parentNode.replaceChild(newCb, cb);
        });

        document.querySelectorAll('.row-checkbox').forEach(cb => {

            cb.addEventListener('change', function () {

                const id = parseInt(this.value);

                if (this.checked) {

                    if (!LocationState.selectedIds.includes(id)) {
                        LocationState.selectedIds.push(id);
                    }

                    this.closest('tr').classList.add('table-active');

                } else {

                    LocationState.selectedIds =
                        LocationState.selectedIds.filter(x => x !== id);

                    this.closest('tr').classList.remove('table-active');
                }

                LocationToolbar.update();

                console.log("selectedIds:", LocationState.selectedIds);
            });
        });

        if (freshSelectAll) {

            freshSelectAll.addEventListener('change', function () {

                document.querySelectorAll('.row-checkbox').forEach(cb => {

                    cb.checked = this.checked;
                    cb.dispatchEvent(new Event('change'));
                });
            });
        }
    },

    reload: function () {
        this.load(
            LocationState.currentPage || 1,
            LocationState.currentSearch || ''
        );
    },

    load: function (page, search) {
        window.location.href =
            `?pageNumber=${page}&search=${encodeURIComponent(search)}`;
    },

    clearSearch: function () {
        this.load(1, '');
    }
};