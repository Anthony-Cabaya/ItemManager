window.VariantTable = {

    initRows() {

        VariantState.clear();

        const rows = document.querySelectorAll(
            ".variant-row"
        );

        rows.forEach(row => {

            const cb = row.querySelector(
                ".variant-select"
            );

            if (!cb) return;

            const newCb = cb.cloneNode(true);
            cb.parentNode.replaceChild(newCb, cb);

            newCb.addEventListener("change",
                function () {

                    const id = parseInt(this.value);

                    const isActive =
                        row.dataset.isActive === "true";

                    if (this.checked) {
                        row.classList.add("table-active");
                    } else {
                        row.classList.remove("table-active");
                    }

                    VariantState.toggle(id, isActive);
                });
        });
    }
};

document.addEventListener("DOMContentLoaded", () => {
    if (window.VariantTable?.initRows)
        VariantTable.initRows();
});