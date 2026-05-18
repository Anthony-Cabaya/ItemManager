window.LocationToolbar = {

    update: function () {
        const editBtn = document.getElementById("btn-edit-location");
        const deleteBtn = document.getElementById("btn-delete-location");

        const count = LocationState.selectedIds.length;

        editBtn.disabled = count !== 1;
        deleteBtn.disabled = count < 1;
    }
};