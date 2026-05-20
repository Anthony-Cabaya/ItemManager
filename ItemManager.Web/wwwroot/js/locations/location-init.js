window.addEventListener("DOMContentLoaded", function () {

    LocationToolbar.update();
    LocationTable.initRows();

    const searchInput = document.getElementById("location-search");
    const clearSearch = document.getElementById("clear-search");

    if (searchInput?.value) {
        clearSearch.style.display = "block";
        LocationState.currentSearch = searchInput.value;
    }

    // Open Modal
    document
        .getElementById("btn-create-location")
        ?.addEventListener("click", LocationModals.openCreate);

    document
        .getElementById("btn-edit-location")
        ?.addEventListener("click", LocationModals.openEdit);

    document
        .getElementById("btn-delete-location")
        ?.addEventListener("click", LocationModals.openDelete);

    // Actions
    document
        .getElementById("btn-confirm-create-location")
        ?.addEventListener("click", LocationModals.submitCreate);

    document
        .getElementById("btn-confirm-edit-location")
        ?.addEventListener("click", LocationModals.submitEdit);

    document
        .getElementById("btn-confirm-delete-location")
        ?.addEventListener("click", LocationModals.submitDelete);

    document
        .getElementById("btn-search")
        ?.addEventListener("click", function () {
            const search = searchInput?.value || "";
            LocationTable.load(1, search);
        });

    searchInput?.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            LocationTable.load(1, this.value);
        }
    });

    searchInput?.addEventListener("input", function () {
        if (clearSearch) {
            clearSearch.style.display = this.value ? "block" : "none";
        }
    });

    clearSearch?.addEventListener("click", function () {
        searchInput.value = "";
        LocationState.currentSearch = "";
        LocationTable.load(1, "");
        this.style.display = "none";
    });

    document
        .getElementById("btn-prev-page")
        ?.addEventListener("click", function () {
            if (this.closest(".disabled")) return;

            LocationTable.load(
                LocationState.currentPage - 1,
                LocationState.currentSearch
            );
        });

    document
        .getElementById("btn-next-page")
        ?.addEventListener("click", function () {
            if (this.closest(".disabled")) return;

            LocationTable.load(
                LocationState.currentPage + 1,
                LocationState.currentSearch
            );
        });

    document.querySelectorAll(".page-number-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            const page = parseInt(this.dataset.page);

            window.location.href =
                `?pageNumber=${page}&search=${encodeURIComponent(LocationState.currentSearch)}`;
        });
    });
});