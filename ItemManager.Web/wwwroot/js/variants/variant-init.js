document.addEventListener("DOMContentLoaded", () => {

    const saveBtn = document.getElementById("btn-save-variants");
    const saveResult = document.getElementById("save-result");
    const container = document.getElementById("matrix-table-container");

    function getItemId() {
        return parseInt(
            document.getElementById("page-item-id")?.value || "0"
        );
    }

    function getAntiForgeryToken() {
        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;
    }

    function showResult(success, message) {
        if (!saveResult) return;

        saveResult.classList.remove("d-none", "alert-success", "alert-danger");
        saveResult.classList.add(success ? "alert-success" : "alert-danger");
        saveResult.textContent = message;
    }

    function updateSelectAllState() {

        const selectAll = document.getElementById("matrix-select-all");
        if (!selectAll || !container) return;

        const checks = container.querySelectorAll(".matrix-check");
        const checked = container.querySelectorAll(".matrix-check:checked").length;

        selectAll.checked = checks.length > 0 && checked === checks.length;
        selectAll.indeterminate = checked > 0 && checked < checks.length;
    }

    function applyRowState(row, checked) {

        const nameCell = row?.querySelector(".variant-name-cell");
        const codeInput = row?.querySelector(".variant-code-input");

        if (!row || !nameCell || !codeInput) return;

        row.style.opacity = checked ? "1" : "0.45";
        row.classList.toggle("bg-light", !checked);

        nameCell.style.textDecoration = checked ? "none" : "line-through";
        codeInput.disabled = !checked;
    }

    document.addEventListener("change", (e) => {

        if (!container || !container.contains(e.target)) return;

        if (e.target?.id === "matrix-select-all") {

            const checks = container.querySelectorAll(".matrix-check");

            checks.forEach(cb => {
                cb.checked = e.target.checked;
                applyRowState(cb.closest("tr"), cb.checked);
            });

            updateSelectAllState();
        }

        if (e.target?.classList.contains("matrix-check")) {

            const row = e.target.closest("tr");
            applyRowState(row, e.target.checked);

            updateSelectAllState();
        }
    });

    if (saveBtn) {

        saveBtn.addEventListener("click", async () => {

            saveResult.classList.add("d-none");

            const itemId = getItemId();
            await window.VariantMatrix?.saveAndRebuild(itemId);
            const rows = window.VariantMatrix?.getCheckedRows() || [];

            if (rows.length === 0) {
                showResult(false, "No variants selected.");
                return;
            }

            const payload = {
                itemID: itemId,
                rows: rows.map(r => ({
                    isChecked: true,
                    variantCode: r.variantCode,
                    variantName: r.variantName,
                    attributeValueIds: r.attributeValueIds
                }))
            };

            try {

                const res = await fetch("/ItemVariant/BulkSave", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": getAntiForgeryToken()
                    },
                    body: JSON.stringify(payload)
                });

                const data = await res.json();

                if (!data.success) {
                    showResult(false, data.message || "Save failed.");
                    return;
                }

                showResult(true, data.message || "Saved successfully.");
                toastSuccess(data.message || "Saved successfully.");

                setTimeout(() => location.reload(), 800);

            } catch (err) {
                console.error(err);
                showResult(false, "Server error while saving.");
            }
        });
    }

    document.querySelectorAll(".variant-tab")
        .forEach(tab => {
            tab.addEventListener("shown.bs.tab", (e) => {

                document.querySelectorAll(".variant-tab")
                    .forEach(t => t.classList.remove("active"));

                e.target.classList.add("active");
            });
        });

});