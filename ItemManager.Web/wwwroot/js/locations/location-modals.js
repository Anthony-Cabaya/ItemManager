window.LocationModals = {

    openCreate: function () {
        document.getElementById("create-location-name").value = "";
        document.getElementById("create-location-description").value = "";
        document.getElementById("create-location-isactive").checked = true;
        document.getElementById("create-location-sort").value = 0;

        openModal("createLocationModal");
    },

    openEdit: function () {
        const id = LocationState.selectedIds[0];
        const row = document.querySelector(`.location-row[data-id='${id}']`);

        document.getElementById("edit-location-id").value = id;
        document.getElementById("edit-location-name").value = row.dataset.name;
        document.getElementById("edit-location-description").value = row.dataset.description;
        document.getElementById("edit-location-isactive").checked = row.dataset.isActive === "true";
        document.getElementById("edit-location-sort").value = row.dataset.sort;

        openModal("editLocationModal");
    },

    openDelete: function () {
        const ids = LocationState.selectedIds;

        const firstRow = document.querySelector(`.location-row[data-id='${ids[0]}']`);

        document.getElementById("delete-location-id").value = ids.join(",");
        document.getElementById("delete-location-name").innerText = firstRow.dataset.name;

        openModal("deleteLocationModal");
    },

    submitCreate: function () {
        const name = document.getElementById("create-location-name").value;

        if (!name) return;

        postJson("/Location/Create", {
            locationName: name,
            description: document.getElementById("create-location-description").value,
            isActive: document.getElementById("create-location-isactive").checked,
            sort: document.getElementById("create-location-sort").value
        }).then(res => {

            if (res.success) {
                closeModal("createLocationModal");
                showToast(res.message, "create");
                LocationTable.load(1, "");
            } else {
                const err = document.getElementById("create-location-error");
                err.innerText = res.message;
                err.classList.remove("d-none");
            }
        });
    },

    submitEdit: function () {
        const name = document.getElementById("edit-location-name").value;

        if (!name) return;

        postJson("/Location/Edit", {
            locationID: document.getElementById("edit-location-id").value,
            locationName: name,
            description: document.getElementById("edit-location-description").value,
            isActive: document.getElementById("edit-location-isactive").checked,
            sort: document.getElementById("edit-location-sort").value
        }).then(res => {

            if (res.success) {
                closeModal("editLocationModal");
                showToast(res.message, "edit");
                LocationTable.load(LocationState.currentPage, LocationState.currentSearch);
            } else {
                const err = document.getElementById("edit-location-error");
                err.innerText = res.message;
                err.classList.remove("d-none");
            }
        });
    },

    submitDelete: function () {

        const ids = document.getElementById("delete-location-id").value.split(",");

        Promise.all(ids.map(id =>
            postJson("/Location/Delete", { id: parseInt(id) })
        )).then(results => {

            const failed = results.find(r => !r.success);

            if (failed) {
                const err = document.getElementById("delete-location-error");
                err.innerText = failed.message;
                err.classList.remove("d-none");
                return;
            }

            closeModal("deleteLocationModal");
            showToast("Deleted", "delete");
            LocationState.selectedIds = [];
            LocationTable.load(1, "");
        });
    }
};