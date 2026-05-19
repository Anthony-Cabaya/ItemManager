window.LocationModals = {

    openCreate: function () {

        document.getElementById('create-location-name').value = '';
        document.getElementById('create-location-isactive').checked = true;
        document.getElementById('create-location-sort').value = 0;
        document.getElementById('create-location-error').classList.add('d-none');

        openModal('createLocationModal');
    },

    openEdit: function () {

        const row = document.querySelector('.location-row.table-active');

        if (!row) {
            return;
        }

        document.getElementById('edit-location-id').value = row.dataset.id;
        document.getElementById('edit-location-name').value = row.dataset.name;
        document.getElementById('edit-location-sort').value = row.dataset.sort;
        document.getElementById('edit-location-isactive').checked = row.dataset.active === 'true';
        document.getElementById('edit-location-error').classList.add('d-none');

        openModal('editLocationModal');
    },

    submitCreate: async function () {

        const name =
            document.getElementById('create-location-name')
                .value
                .trim();

        if (!name) {

            const error =
                document.getElementById('create-location-error');

            error.textContent = 'Location name is required.';
            error.classList.remove('d-none');

            return;
        }

        const response = await postJson('/Location/Create', {
            locationName: name,
            isActive: document.getElementById('create-location-isactive').checked,
            sort: document.getElementById('create-location-sort').value
        });

        if (response.success) {

            closeModal('createLocationModal');

            showToast(response.message, 'create');

            if (window.LocationTable) {
                LocationTable.reload();
            }

            return;
        }

        const error =
            document.getElementById('create-location-error');

        error.textContent = response.message;
        error.classList.remove('d-none');
    },

    submitEdit: async function () {

        const id =
            document.getElementById('edit-location-id').value;

        const name =
            document.getElementById('edit-location-name')
                .value
                .trim();

        if (!name) {

            const error =
                document.getElementById('edit-location-error');

            error.textContent = 'Location name is required.';
            error.classList.remove('d-none');

            return;
        }

        const response = await postJson('/Location/Edit', {
            locationId: id,
            locationName: name,
            isActive: document.getElementById('edit-location-isactive').checked,
            sort: document.getElementById('edit-location-sort').value
        });

        if (response.success) {

            closeModal('editLocationModal');

            showToast(response.message, 'edit');

            if (window.LocationTable) {
                LocationTable.reload();
            }

            return;
        }

        const error =
            document.getElementById('edit-location-error');

        error.textContent = response.message;
        error.classList.remove('d-none');
    }
};