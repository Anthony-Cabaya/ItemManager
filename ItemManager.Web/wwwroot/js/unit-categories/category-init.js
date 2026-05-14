document.addEventListener(
    "DOMContentLoaded",
    async () => {

        const pageData =
            document.getElementById("pageData");

        CategoryState.isAdmin =
            pageData.dataset.isAdmin === "true";

        initCategorySearch();

        await loadCategories();

        document.getElementById("btnCreate")
            ?.addEventListener("click",
                () => openCreateModal());

        document.getElementById("btnEdit")
            ?.addEventListener("click", () => {

                if (CategoryState.selectedIds.length !== 1)
                    return;

                openEditModal(
                    CategoryState.selectedIds[0]);
            });

        document.getElementById("btnDelete")
            ?.addEventListener("click",
                () => openDeleteModal());

        document.getElementById("btnCreateSave")
            ?.addEventListener("click",
                () => saveCreate());

        document.getElementById("btnEditSave")
            ?.addEventListener("click",
                () => saveEdit());

        document.getElementById("btnDeleteConfirm")
            ?.addEventListener("click",
                () => deleteCategories());
    });