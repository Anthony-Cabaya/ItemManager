document.addEventListener("DOMContentLoaded", () => {

    const editError = document.getElementById(
        "edit-variant-error");

    const deleteError = document.getElementById(
        "delete-variant-error");

    const editBtn = document.getElementById(
        "btn-update-variant");

    if (editBtn) {
        editBtn.addEventListener("click", async () => {

            editError.classList.add("d-none");
            editError.textContent = "";

            const payload = {
                itemVariantID: parseInt(document.getElementById("variant-edit-id").value),
                itemID: parseInt(document.getElementById("variant-edit-item-id").value),
                variantCode: document.getElementById("variant-edit-code").value,
                variantName: document.getElementById("variant-edit-name").value,
                isActive: document.getElementById("variant-edit-is-active").checked,
                sort: parseInt(document.getElementById("variant-edit-sort").value || "0")
            };

            try {

                const res = await fetch("/ItemVariant/Edit", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": getAntiForgeryToken()
                    },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();

                if (!data.success) {
                    editError.textContent =
                        data.message || "Update failed";

                    editError.classList.remove("d-none");
                    return;
                }

                toastSuccess(
                    data.message || "Updated successfully");

                bootstrap.Modal.getInstance(
                    document.getElementById("editVariantModal")
                )?.hide();

                const tableContainer =
                    document.getElementById(
                        "variant-table-container");

                if (tableContainer) {

                    const itemId = parseInt(
                        document.getElementById("page-item-id")
                            ?.value || "0");

                    fetch(`/ItemVariant/GetByItem?itemId=${itemId}`)
                        .then(r => r.text())
                        .then(html => {

                            tableContainer.innerHTML = html;

                            if (window.VariantTable?.initRows)
                                window.VariantTable.initRows();

                            if (window.VariantState)
                                VariantState.selectedIds.clear();

                            if (window.VariantState)
                                VariantState.updateUI();
                        });
                }

            } catch (err) {

                editError.textContent = "Server error";
                editError.classList.remove("d-none");
            }
        });
    }

    const deleteBtn = document.getElementById(
        "btn-confirm-delete-variant");

    if (deleteBtn) {

        deleteBtn.addEventListener("click", async () => {

            deleteError.classList.add("d-none");
            deleteError.textContent = "";

            const variantId = parseInt(
                document.getElementById(
                    "delete-variant-id").value);

            try {

                const res = await fetch(
                    "/ItemVariant/Delete",
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken":
                                getAntiForgeryToken()
                        },
                        body: JSON.stringify({
                            variantId
                        })
                    });

                const data = await res.json();

                if (!data.success) {

                    deleteError.textContent =
                        data.message || "Delete failed";

                    deleteError.classList.remove("d-none");
                    return;
                }

                toastSuccess(
                    data.message || "Deleted successfully");

                bootstrap.Modal.getInstance(
                    document.getElementById(
                        "deleteVariantModal")
                )?.hide();

                const tableContainer =
                    document.getElementById(
                        "variant-table-container");

                if (tableContainer) {

                    const itemId = parseInt(
                        document.getElementById("page-item-id")
                            ?.value || "0");

                    fetch(`/ItemVariant/GetByItem?itemId=${itemId}`)
                        .then(r => r.text())
                        .then(html => {

                            tableContainer.innerHTML = html;

                            if (window.VariantTable?.initRows)
                                window.VariantTable.initRows();

                            if (window.VariantState)
                                VariantState.selectedIds.clear();

                            if (window.VariantState)
                                VariantState.updateUI();
                        });
                }

            } catch (err) {

                deleteError.textContent = "Server error";
                deleteError.classList.remove("d-none");
            }
        });
    }

});