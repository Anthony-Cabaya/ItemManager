window.LocationTable = {

    initRows: function () {
        const checkboxes = document.querySelectorAll('.row-checkbox');
        const selectAll = document.getElementById('selectAll');

        checkboxes.forEach(cb => {
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
            });
        });

        if (selectAll) {
            selectAll.addEventListener('change', function () {
                checkboxes.forEach(cb => {
                    cb.checked = this.checked;
                    cb.dispatchEvent(new Event('change'));
                });
            });
        }
    }
};