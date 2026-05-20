window.LocationModals = {

    openCreate: function () {
        document.getElementById('create-location-name').value = '';
        document.getElementById('create-location-isactive').checked = true;
        document.getElementById('create-location-sort').value = 0;

        const err = document.getElementById('create-location-error');
        if (err) err.classList.add('d-none');

        openModal('createLocationModal');
    },

    openEdit: function () {

        const row = document.querySelector('.location-row.table-active');
        if (!row) return;

        document.getElementById('edit-location-id').value = row.dataset.id;
        document.getElementById('edit-location-name').value = row.dataset.name;
        document.getElementById('edit-location-sort').value = row.dataset.sort;

        const isActive = (row.dataset.isActive || "").toLowerCase() === "true";
        document.getElementById('edit-location-isactive').checked = isActive;

        const err = document.getElementById('edit-location-error');
        if (err) err.classList.add('d-none');

        openModal('editLocationModal');
    },

    openDelete: function () {

        const ids = LocationState.selectedIds;
        if (!ids.length) return;

        const rows = ids.map(id =>
            document.querySelector(`.location-row[data-id='${id}']`)
        ).filter(Boolean);

        const names = rows.map(r => r.dataset.name);

        const idInput = document.getElementById('delete-location-id');
        const nameEl = document.getElementById('delete-location-name');
        const errorEl = document.getElementById('delete-location-error');

        if (!idInput || !nameEl) return;

        idInput.value = ids.join(',');

        if (names.length === 1) {
            nameEl.innerText = names[0];
        } else if (names.length <= 3) {
            nameEl.innerText = names.join(', ');
        } else {
            nameEl.innerText = `${names.slice(0, 2).join(', ')} and ${names.length - 2} more`;
        }

        if (errorEl) errorEl.classList.add('d-none');

        openModal('deleteLocationModal');
    },

    submitCreate: async function () {

        const name = document.getElementById('create-location-name').value.trim();

        if (!name) return;

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await postJson('/Location/Create', {
            LocationName: name,
            IsActive: document.getElementById('create-location-isactive').checked,
            Sort: parseInt(document.getElementById('create-location-sort').value)
        }, token);

        if (response.success) {
            closeModal('createLocationModal');
            showToast(response.message, 'create');
            LocationTable.reload();
            return;
        }

        const err = document.getElementById('create-location-error');
        if (err) {
            err.textContent = response.message;
            err.classList.remove('d-none');
        }
    },

    submitEdit: async function () {

        const id = document.getElementById('edit-location-id').value;
        const name = document.getElementById('edit-location-name').value.trim();

        if (!name) return;

        const response = await postJson('/Location/Edit', {
            LocationID: parseInt(id),
            LocationName: name,
            IsActive: document.getElementById('edit-location-isactive').checked,
            Sort: parseInt(document.getElementById('edit-location-sort').value)
        });

        if (response.success) {
            closeModal('editLocationModal');
            showToast(response.message, 'edit');
            LocationTable.reload();
            return;
        }

        const err = document.getElementById('edit-location-error');
        if (err) {
            err.textContent = response.message;
            err.classList.remove('d-none');
        }
    },

    submitDelete: async function () {

        const ids = document.getElementById('delete-location-id')
            .value
            .split(',')
            .map(x => parseInt(x));

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await postJson('/Location/Delete', {
            ids: ids
        }, token);

        if (!response.success) {
            const err = document.getElementById('delete-location-error');
            if (err) {
                err.textContent = response.message;
                err.classList.remove('d-none');
            }
            return;
        }

        closeModal('deleteLocationModal');
        showToast(response.message, 'delete');

        LocationState.selectedIds = [];
        LocationTable.reload();
    }
};