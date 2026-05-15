window.UnitModals = {

    openCreate: function () {

        document.getElementById('create-unit-name').value = '';
        document.getElementById('create-unit-abbr').value = '';
        document.getElementById('create-unit-sort').value = 0;

        document.getElementById('create-unit-category-id').value =
            UnitState.selectedCategoryId || '';

        document.getElementById('create-error').classList.add('d-none');

        openModal('createUnitModal');
    },

    openEdit: function () {

        if (!UnitState.selectedUnitId) return;

        const row = document.querySelector('.unit-row.table-active');
        if (!row) return;

        const isSystem = row.dataset.isSystem === 'True';

        document.getElementById('edit-unit-id').value = row.dataset.id;
        document.getElementById('edit-unit-name').value = row.dataset.name;
        document.getElementById('edit-unit-abbr').value = row.dataset.abbreviation;
        document.getElementById('edit-unit-sort').value = row.dataset.sort;
        document.getElementById('edit-unit-category-id').value = row.dataset.categoryId;
        document.getElementById('edit-is-system').value = row.dataset.isSystem;

        const name = document.getElementById('edit-unit-name');
        const abbr = document.getElementById('edit-unit-abbr');
        const badge = document.getElementById('edit-system-badge');

        if (isSystem) {
            name.readOnly = true;
            abbr.readOnly = true;
            badge.classList.remove('d-none');
        } else {
            name.readOnly = false;
            abbr.readOnly = false;
            badge.classList.add('d-none');
        }

        document.getElementById('edit-error').classList.add('d-none');

        openModal('editUnitModal');
    },

    openDelete: function () {

        if (!UnitState.selectedUnitId) return;

        document.getElementById('delete-unit-id').value = UnitState.selectedUnitId;
        document.getElementById('delete-unit-name').textContent = UnitState.selectedUnitName;

        document.getElementById('delete-error').classList.add('d-none');

        openModal('deleteUnitModal');
    },

    submitCreate: function () {

        const name = document.getElementById('create-unit-name').value.trim();
        const abbr = document.getElementById('create-unit-abbr').value.trim();
        const sort = document.getElementById('create-unit-sort').value;
        const categoryId = document.getElementById('create-unit-category-id').value;

        const error = document.getElementById('create-error');

        if (!name || !abbr) {
            error.textContent = 'Please fill in all required fields.';
            error.classList.remove('d-none');
            return;
        }

        postJson('/Unit/Create', {
            UnitName: name,
            Abbreviation: abbr,
            UnitCategoryID: categoryId,
            Sort: sort,
        })
            .then(res => {
                if (res.success) {
                    closeModal('createUnitModal');
                    showToast(res.message, 'create');

                    UnitTable.loadByCategory(UnitState.selectedCategoryId);
                } else {
                    error.textContent = res.message;
                    error.classList.remove('d-none');
                }
            })
            .catch(() => {
                error.textContent = 'An unexpected error occurred.';
                error.classList.remove('d-none');
            });
    },

    submitEdit: function () {

        const id = document.getElementById('edit-unit-id').value;
        const name = document.getElementById('edit-unit-name').value.trim();
        const abbr = document.getElementById('edit-unit-abbr').value.trim();
        const sort = document.getElementById('edit-unit-sort').value;
        const categoryId = document.getElementById('edit-unit-category-id').value;
        const isSystem = document.getElementById('edit-is-system').value === 'True';

        const error = document.getElementById('edit-error');

        if (!isSystem && (!name || !abbr)) {
            error.textContent = 'Please fill in all required fields.';
            error.classList.remove('d-none');
            return;
        }

        postJson('/Unit/Edit', {
            UnitID: id,
            UnitName: name,
            Abbreviation: abbr,
            UnitCategoryID: categoryId,
            Sort: sort
        })
            .then(res => {
                if (res.success) {
                    closeModal('editUnitModal');
                    showToast(res.message, 'edit');

                    UnitTable.loadByCategory(UnitState.selectedCategoryId);
                } else {
                    error.textContent = res.message;
                    error.classList.remove('d-none');
                }
            })
            .catch(() => {
                error.textContent = 'An unexpected error occurred.';
                error.classList.remove('d-none');
            });
    },

    submitDelete: function () {

        const id = document.getElementById('delete-unit-id').value;
        const error = document.getElementById('delete-error');

        postJson('/Unit/Delete', {
            id: id
        })
            .then(res => {
                if (res.success) {
                    closeModal('deleteUnitModal');
                    showToast(res.message, 'delete');

                    UnitState.selectedUnitId = null;
                    UnitState.selectedUnitName = '';
                    UnitState.selectedUnitIsSystem = '';

                    UnitToolbar.update();

                    UnitTable.loadByCategory(UnitState.selectedCategoryId);
                } else {
                    error.textContent = res.message;
                    error.classList.remove('d-none');
                }
            })
            .catch(() => {
                error.textContent = 'An unexpected error occurred.';
                error.classList.remove('d-none');
            });
    }
};