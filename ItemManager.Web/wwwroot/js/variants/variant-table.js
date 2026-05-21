document.addEventListener("DOMContentLoaded", () => {

    // Checkbox selection handling
    document.addEventListener("change", (e) => {

        if (e.target.classList.contains("variant-checkbox")) {
            const row = e.target.closest(".variant-row");
            const id = parseInt(e.target.value);

            if (e.target.checked) {
                VariantState.selectedIds.add(id);
                row.classList.add("table-active");
            } else {
                VariantState.selectedIds.delete(id);
                row.classList.remove("table-active");
            }

            VariantState.updateUI();
        }

        // Active toggle switch
        if (e.target.classList.contains("variant-toggle")) {

            const variantId = parseInt(e.target.dataset.variantId);
            const isActive = e.target.checked;

            fetch("/ItemVariant/SetActive", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getAntiForgeryToken()
                },
                body: JSON.stringify({
                    variantId: variantId,
                    isActive: isActive
                })
            })
                .then(r => r.json())
                .then(res => {
                    if (!res.success) {
                        e.target.checked = !isActive;
                        toastError(res.message || "Failed to update status.");
                        return;
                    }

                    const row = e.target.closest(".variant-row");
                    row.dataset.isActive = isActive;

                    const statusCell = row.querySelector("td:nth-child(8)");
                    if (statusCell) {
                        statusCell.innerHTML = isActive
                            ? `<span class="badge bg-success">Active</span>`
                            : `<span class="badge bg-secondary">Inactive</span>`;
                    }

                    toastSuccess(res.message);
                })
                .catch(() => {
                    e.target.checked = !isActive;
                    toastError("Server error.");
                });
        }
    });

});