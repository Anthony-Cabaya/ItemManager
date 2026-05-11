document.addEventListener("DOMContentLoaded", async () => {

    const pageData =
        document.getElementById("pageData");

    ItemTypeState.isAdmin =
        pageData.dataset.isAdmin === "true";

    initItemTypeSearch();

    await loadItemTypes();

    document.getElementById("btnCreate")
        .addEventListener("click", () =>
            openCreateModal());

    document.getElementById("btnEdit")
        .addEventListener("click", () => {

            if (ItemTypeState.selectedIds.length === 1) {

                openEditModal(
                    ItemTypeState.selectedIds[0]);
            }
        });

    document.getElementById("btnDelete")
        .addEventListener("click", () =>
            openDeleteModal());

    document.getElementById("btnCreateSave")
        .addEventListener("click", () =>
            saveCreate());

    document.getElementById("btnEditSave")
        .addEventListener("click", () =>
            saveEdit());

    document.getElementById("btnDeleteConfirm")
        .addEventListener("click", () =>
            deleteItemTypes());
});