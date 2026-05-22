window.VariantState = {

    selectedIds: new Set(),
    selectedStatuses: {},

    clear() {
        this.selectedIds.clear();
        this.selectedStatuses = {};
        this.updateUI();
    },

    toggle(id, isActive) {

        if (this.selectedIds.has(id)) {

            this.selectedIds.delete(id);
            delete this.selectedStatuses[id];

        } else {

            this.selectedIds.add(id);
            this.selectedStatuses[id] = isActive;
        }

        this.updateUI();
    },

    getSelected() {
        return Array.from(this.selectedIds);
    },

    updateUI() {

        const count = this.selectedIds.size;
        const ids = this.getSelected();

        const btnEdit = document.getElementById("btn-edit-variant");
        const btnDelete = document.getElementById("btn-delete-variant");
        const btnActivate = document.getElementById("btn-activate-variant");
        const btnDeactivate = document.getElementById("btn-deactivate-variant");
        const label = document.getElementById("selectedCount");

        if (btnEdit)
            btnEdit.disabled = count !== 1;

        if (btnDelete)
            btnDelete.disabled = count === 0;

        if (count > 0) {

            const statuses = ids.map(
                id => this.selectedStatuses[id]
            );

            const hasInactive = statuses.includes(false);
            const hasActive = statuses.includes(true);

            if (btnActivate) {
                btnActivate.style.display =
                    hasInactive ? "" : "none";
            }

            if (btnDeactivate) {
                btnDeactivate.style.display =
                    hasActive ? "" : "none";
            }

        } else {

            if (btnActivate)
                btnActivate.style.display = "none";

            if (btnDeactivate)
                btnDeactivate.style.display = "none";
        }

        if (label) {

            label.textContent =
                count > 0
                    ? `${count} selected`
                    : "";
        }
    }
};