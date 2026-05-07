document.addEventListener("DOMContentLoaded",
    async () => {

        const pageData =
            document.getElementById("pageData");
        ItemState.isAdmin =
            pageData.dataset.isAdmin === "true";

        initColumns(
            "item-visible-cols-v1",
            ["col-2", "col-3", "col-4", "col-5", "col-6",
                "col-7", "col-8", "col-9", "col-10", "col-11"]
        );

        initSearch();
        initFilters();
        await loadItems();

        document.getElementById("btnCreate")
            .addEventListener("click",
                () => openCreateModal());

        document.getElementById("btnEdit")
            .addEventListener("click", () => {
                if (ItemState.selectedIds.length === 1)
                    openEditModal(ItemState.selectedIds[0]);
            });

        document.getElementById("btnDelete")
            .addEventListener("click",
                () => openDeleteModal());

        document.getElementById("btnConversions")
            .addEventListener("click", () => {
                if (ItemState.lastSelectedItemId)
                    window.location.href =
                        `/Item/Conversions/` +
                        `${ItemState.lastSelectedItemId}`;
            });

        document.getElementById("btnCreateSave")
            .addEventListener("click",
                () => saveCreate());

        document.getElementById("btnEditSave")
            .addEventListener("click",
                () => saveEdit());

        document.getElementById("btnDeleteConfirm")
            .addEventListener("click",
                () => deleteItems());

        document.getElementById("btnColumns")
            .addEventListener("click", () =>
                toggleColumnPanel("columnPanel"));

        document.querySelectorAll(".col-toggle")
            .forEach(cb => {
                cb.addEventListener("change", () =>
                    saveColumnPrefs("item-visible-cols-v1"));
            });

        ItemModals.initModalListeners();

    });