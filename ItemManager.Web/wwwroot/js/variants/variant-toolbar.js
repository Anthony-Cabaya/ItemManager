document.addEventListener("DOMContentLoaded", () => {

    const btnAdd = document.getElementById("btn-add-variant");
    const btnCancel = document.getElementById("btn-cancel-add");
    const btnEdit = document.getElementById("btn-edit-variant");
    const btnDelete = document.getElementById("btn-delete-variant");
    const btnActivate = document.getElementById("btn-activate-variant");
    const btnDeactivate = document.getElementById("btn-deactivate-variant");

    if (btnAdd) {
        btnAdd.addEventListener("click", () => {

            const section = document.getElementById("add-variant-section");

            if (section) {
                section.style.display = "block";

                section.scrollIntoView({
                    behavior: "smooth"
                });
            }
        });
    }

    if (btnCancel) {
        btnCancel.addEventListener("click", () => {

            const section =
                document.getElementById(
                    "add-variant-section"
                );

            if (section)
                section.style.display = "none";
        });
    }

    // EDIT
    if (btnEdit) {
        btnEdit.addEventListener("click", () => {

            const selected =VariantState.getSelected();

            if (selected.length !== 1)
                return;

            const id = selected[0];

            const row = document.querySelector(
                `.variant-row[data-id="${id}"]`
            );

            if (!row) return;

            document.getElementById("edit-variant-error")?.classList.add("d-none");
            document.getElementById("variant-edit-id").value = id;
            document.getElementById("variant-edit-item-id").value = row.dataset.itemId;
            document.getElementById("variant-edit-code").value = row.dataset.variantCode;
            document.getElementById("variant-edit-name").value = row.dataset.variantName;
            document.getElementById("variant-edit-is-active").checked =
                row.dataset.isActive === "true";

            document.getElementById("variant-edit-sort").value = row.dataset.sort;

            new bootstrap.Modal(
                document.getElementById("editVariantModal")
            ).show();
        });
    }

    if (btnDelete) {
        btnDelete.addEventListener("click", () => {

            const selected =
                VariantState.getSelected();

            if (selected.length === 0)
                return;

            const id = selected[0];

            const row = document.querySelector(`.variant-row[data-id="${id}"]`);

            if (!row) return;

            document.getElementById("delete-variant-error")?.classList.add("d-none");
            document.getElementById("delete-variant-id").value = id;
            document.getElementById("delete-variant-name").textContent = row.dataset.variantName;

            new bootstrap.Modal(
                document.getElementById("deleteVariantModal")
            ).show();
        });
    }

    if (btnActivate) {
        btnActivate.addEventListener(
            "click",
            async () => {

                const ids =
                    VariantState.getSelected()
                        .filter(id =>
                            VariantState
                                .selectedStatuses[id] === false
                        );

                for (const id of ids) {

                    await fetch(
                        "/ItemVariant/SetActive",
                        {
                            method: "POST",
                            headers: {
                                "Content-Type":
                                    "application/json"
                            },
                            body: JSON.stringify({
                                variantId: id,
                                isActive: true
                            })
                        });
                }

                reloadVariantTable();
            });
    }

    if (btnDeactivate) {
        btnDeactivate.addEventListener(
            "click",
            async () => {

                const ids =
                    VariantState.getSelected()
                        .filter(id =>
                            VariantState
                                .selectedStatuses[id] === true
                        );

                for (const id of ids) {

                    await fetch(
                        "/ItemVariant/SetActive",
                        {
                            method: "POST",
                            headers: {
                                "Content-Type":
                                    "application/json"
                            },
                            body: JSON.stringify({
                                variantId: id,
                                isActive: false
                            })
                        });
                }

                reloadVariantTable();
            });
    }

});

function reloadVariantTable() {

    const container = document.getElementById(
            "variant-table-container"
        );

    const itemId = parseInt(document.getElementById(
            "page-item-id"
        )?.value || "0"
    );

    if (!container || !itemId)
        return;

    fetch(`/ItemVariant/GetByItem?itemId=${itemId}`)
        .then(r => r.text())
        .then(html => {

            container.innerHTML = html;

            if (window.VariantTable?.initRows)
                VariantTable.initRows();

            if (window.VariantState)
                VariantState.clear();
        });
}