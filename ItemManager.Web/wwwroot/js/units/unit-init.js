document.addEventListener('DOMContentLoaded', function () {

    UnitCategory.initCategoryList();

    document.getElementById('btn-create-unit')
        .addEventListener('click', UnitModals.openCreate);

    document.getElementById('btn-edit-unit')
        .addEventListener('click', UnitModals.openEdit);

    document.getElementById('btn-delete-unit')
        .addEventListener('click', UnitModals.openDelete);

    document.getElementById('btn-save-unit')
        .addEventListener('click', UnitModals.submitCreate);

    document.getElementById('btn-update-unit')
        .addEventListener('click', UnitModals.submitEdit);

    document.getElementById('btn-confirm-delete-unit')
        .addEventListener('click', UnitModals.submitDelete);

    document.getElementById('btn-search-unit')
        .addEventListener('click', function () {

            UnitTable.filterRows(
                document.getElementById('unit-search').value
            );
        });

    document.getElementById('unit-search')
        .addEventListener('keydown', function (e) {

            if (e.key === 'Enter') {
                UnitTable.filterRows(this.value);
            }
        });

    document.getElementById('unit-search-clear')
        .addEventListener('click', function () {

            UnitTable.clearSearch();
        });

    document.getElementById('unit-search')
        .addEventListener('input', function () {

            const clear =
                document.getElementById('unit-search-clear');

            if (this.value.length === 0) {
                UnitTable.clearSearch();
            }
            else {
                clear.style.display = 'block';
            }
        });
});