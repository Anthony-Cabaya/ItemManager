document.addEventListener(
    "DOMContentLoaded",
    async () => {

        const pageData =
            document.getElementById("pageData");

        SubTypeState.isAdmin =
            pageData.dataset.isAdmin === "true";

        initSubTypeSearch();
        initSubTypeFilters();

        await loadSubTypes();

        const btnCreate =
            document.getElementById("btnCreate");

        const btnEdit =
            document.getElementById("btnEdit");

        const btnDelete =
            document.getElementById("btnDelete");

        const btnCreateSave =
            document.getElementById("btnCreateSave");

        const btnEditSave =
            document.getElementById("btnEditSave");

        const btnDeleteConfirm =
            document.getElementById(
                "btnDeleteConfirm");

        if (btnCreate) {

            btnCreate.addEventListener("click",
                () => {
                    SubTypeModals.openCreateModal();
                });
        }

        if (btnEdit) {

            btnEdit.addEventListener("click",
                () => {

                    if (SubTypeState.selectedIds.length !== 1) {
                        return;
                    }

                    SubTypeModals.openEditModal(
                        SubTypeState.selectedIds[0]);
                });
        }

        if (btnDelete) {

            btnDelete.addEventListener("click",
                () => {
                    SubTypeModals.openDeleteModal();
                });
        }

        if (btnCreateSave) {

            btnCreateSave.addEventListener("click",
                async () => {
                    await SubTypeModals.saveCreate();
                });
        }

        if (btnEditSave) {

            btnEditSave.addEventListener("click",
                async () => {
                    await SubTypeModals.saveEdit();
                });
        }

        if (btnDeleteConfirm) {

            btnDeleteConfirm.addEventListener("click",
                async () => {
                    await SubTypeModals.deleteSubTypes();
                });
        }
    });