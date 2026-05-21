window.VariantState = {
    selectedIds: new Set(),

    clear() {
        this.selectedIds.clear();
        this.updateUI();
    },

    toggle(id) {
        if (this.selectedIds.has(id)) {
            this.selectedIds.delete(id);
        } else {
            this.selectedIds.add(id);
        }
        this.updateUI();
    },

    getSelected() {
        return Array.from(this.selectedIds);
    },

    updateUI() {
        const count = this.selectedIds.size;

        const btnEdit = document.getElementById("btn-edit-variant");
        const btnDelete = document.getElementById("btn-delete-variant");
        const label = document.getElementById("selectedCount");

        if (btnEdit) btnEdit.disabled = count !== 1;
        if (btnDelete) btnDelete.disabled = count === 0;

        if (label) {
            label.textContent = count > 0 ? `${count} selected` : "";
        }
    }
};