document.addEventListener("DOMContentLoaded", () => {

    const btnAdd = document.getElementById("btn-add-variant");
    const btnCancel = document.getElementById("btn-cancel-add");
    const btnEdit = document.getElementById("btn-edit-variant");
    const btnDelete = document.getElementById("btn-delete-variant");
    const btnActivate = document.getElementById("btn-activate-variant");
    const btnDeactivate = document.getElementById("btn-deactivate-variant");

    window.reloadVariantTable = async function () {

        const container =
            document.getElementById("variant-table-container");

        const itemId = parseInt(
            document.getElementById("page-item-id")?.value || "0"
        );

        if (!container || !itemId)
            return;

        try {

            const res = await fetch(
                `/ItemVariant/GetByItem?itemId=${itemId}`
            );

            const html = await res.text();

            container.innerHTML = html;
            window.VariantTable?.initRows();
            window.VariantState?.clear();
            window.VariantState?.updateUI();

        } catch (err) {

            console.error(
                "Variant table reload failed:",
                err
            );
        }
    };

    if (btnAdd) {

        btnAdd.addEventListener("click", () => {

            const errorDiv =
                document.getElementById(
                    "add-variant-error");

            if (errorDiv) {

                errorDiv.textContent = "";
                errorDiv.style.visibility = "hidden";
                errorDiv.style.padding = "0";
                errorDiv.style.margin = "0";
                errorDiv.style.border = "none";
                errorDiv.style.minHeight = "0";
            }

            document.getElementById("add-variant-code").value = "";
            document.getElementById("add-variant-name").value = "";
            document.getElementById("add-variant-attributes").value = "";
            document.getElementById("add-variant-is-active").checked = true;

            const itemId = parseInt(
                document.getElementById("page-item-id")
                    ?.value || "0");

            document.getElementById(
                "add-variant-item-id")
                .value = itemId;

            const modalEl =
                document.getElementById(
                    "addVariantModal");

            const modal =
                new bootstrap.Modal(modalEl);

            modalEl.addEventListener(
                "shown.bs.modal",

                async function handler() {

                    modalEl.removeEventListener(
                        "shown.bs.modal",
                        handler);

                    if (itemId > 0) {

                        try {

                            const res = await fetch(
                                `/ItemVariant/GetNextVariantCode` +
                                `?itemId=${itemId}`);

                            const data =
                                await res.json();

                            if (data.success &&
                                data.data?.code) {

                                document.getElementById(
                                    "add-variant-code")
                                    .value =
                                    data.data.code;
                            }

                        } catch { }
                    }
                });

            modal.show();
        });
    }

    if (btnEdit) {

        btnEdit.addEventListener("click", () => {

            const selected =
                VariantState.getSelected();

            if (selected.length !== 1)
                return;

            const id = selected[0];

            const row = document.querySelector(
                `.variant-row[data-id="${id}"]`
            );

            if (!row)
                return;

            const editError =
                document.getElementById(
                    "edit-variant-error");

            if (editError) {

                editError.textContent = "";
                editError.style.visibility = "hidden";
                editError.style.padding = "0";
                editError.style.margin = "0";
                editError.style.border = "none";
                editError.style.minHeight = "0";
            }

            document.getElementById(
                "variant-edit-id").value =
                id;

            document.getElementById(
                "variant-edit-item-id").value =
                row.dataset.itemId;

            document.getElementById(
                "variant-edit-code").value =
                row.dataset.variantCode;

            document.getElementById(
                "variant-edit-original-code").value =
                row.dataset.variantCode;

            document.getElementById(
                "variant-edit-name").value =
                row.dataset.variantName;

            document.getElementById(
                "variant-edit-attributes").value =
                row.dataset.attributes ?? "";

            document.getElementById(
                "variant-edit-is-active").checked =
                row.dataset.isActive === "true";

            new bootstrap.Modal(
                document.getElementById(
                    "editVariantModal")
            ).show();
        });
    }

    if (btnDelete) {

        btnDelete.addEventListener("click", () => {

            const selected =
                VariantState.getSelected();

            if (selected.length === 0)
                return;

            document.getElementById(
                "delete-variant-id")
                .value = selected.join(",");

            const names = selected.map(id => {

                const row = document.querySelector(
                    `.variant-row[data-id="${id}"]`
                );

                return row?.dataset.variantName ?? id;

            }).join(", ");

            document.getElementById(
                "delete-variant-name")
                .textContent =
                selected.length === 1
                    ? names
                    : `${selected.length} variants`;

            const deleteError =
                document.getElementById(
                    "delete-variant-error");

            if (deleteError) {

                deleteError.textContent = "";
                deleteError.style.visibility = "hidden";
                deleteError.style.padding = "0";
                deleteError.style.margin = "0";
                deleteError.style.border = "none";
                deleteError.style.minHeight = "0";
            }

            new bootstrap.Modal(
                document.getElementById(
                    "deleteVariantModal")
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

                window.reloadVariantTable();
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

                window.reloadVariantTable();
            });
    }

});