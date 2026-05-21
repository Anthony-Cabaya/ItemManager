document.addEventListener("DOMContentLoaded", () => {

    const btnEdit = document.getElementById("btn-edit-variant");
    const btnDelete = document.getElementById("btn-delete-variant");

    if (btnEdit) {
        btnEdit.addEventListener("click", async () => {

            const selected = VariantState.getSelected();
            if (selected.length !== 1) return;

            const id = selected[0];

            const row = document.querySelector(`.variant-row input[value="${id}"]`)?.closest(".variant-row");
            if (!row) return;

            document.getElementById("variant-edit-id").value = id;
            document.getElementById("variant-edit-item-id").value = row.dataset.itemId;
            document.getElementById("variant-edit-code").value = row.dataset.variantCode;
            document.getElementById("variant-edit-name").value = row.dataset.variantName;
            document.getElementById("variant-edit-is-active").checked = row.dataset.isActive === "true";
            document.getElementById("variant-edit-sort").value = row.dataset.sort;

            const modal = new bootstrap.Modal(document.getElementById("editVariantModal"));
            modal.show();
        });
    }

    if (btnDelete) {
        btnDelete.addEventListener("click", async () => {

            const selected = VariantState.getSelected();
            if (selected.length === 0) return;

            const id = selected[0];

            const row = document.querySelector(`.variant-row input[value="${id}"]`)?.closest(".variant-row");
            if (!row) return;

            document.getElementById("delete-variant-id").value = id;
            document.getElementById("delete-variant-name").textContent = row.dataset.variantName;

            const modal = new bootstrap.Modal(document.getElementById("deleteVariantModal"));
            modal.show();
        });
    }

});