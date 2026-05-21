document.addEventListener("DOMContentLoaded", () => {

    const builder = document.getElementById("attribute-builder");
    const template = document.getElementById("attribute-row-template");
    const valueTemplate = document.getElementById("value-input-template");
    const addBtn = document.getElementById("btn-add-attribute");

    function getAttributeCount() {
        return builder.querySelectorAll(".attribute-card").length;
    }

    function updateAddButtonState() {
        if (!addBtn) return;
        addBtn.disabled = getAttributeCount() >= 3;
    }

    function rebuildMatrix() {
        if (window.VariantMatrix?.rebuild) {
            window.VariantMatrix.rebuild();
        }
        updateAddButtonState();
    }

    function createAttributeRow() {
        const clone = template.firstElementChild.cloneNode(true);

        const valuesContainer = clone.querySelector(".values-container");
        const addValueBtn = clone.querySelector(".add-value");
        const removeBtn = clone.querySelector(".remove-attribute");

        clone.classList.add("attribute-card");

        addValueBtn.addEventListener("click", () => {
            const inputClone = valueTemplate.firstElementChild.cloneNode(true);

            const labelInput = inputClone.querySelector(".value-label");
            const okBtn = inputClone.querySelector(".confirm-value");
            const cancelBtn = inputClone.querySelector(".cancel-value");

            okBtn?.addEventListener("click", () => {

                const label = labelInput?.value?.trim();
                if (!label) return;

                const badge = document.createElement("span");
                badge.className =
                    "badge bg-secondary d-inline-flex " +
                    "align-items-center me-2 mb-2 p-2";
                badge.dataset.valueLabel = label;
                badge.dataset.valueId = "";

                badge.innerHTML = `
                    <span class="value-text">${label}</span>
                    <button type="button"
                            class="btn-close btn-close-white ms-2"
                            style="font-size:0.5rem;">
                    </button>
                  `;

                badge.querySelector(".btn-close")
                    .addEventListener("click", (e) => {
                        e.stopPropagation();
                        badge.remove();
                        rebuildMatrix();
                    });

                valuesContainer.appendChild(badge);
                inputClone.remove();
                rebuildMatrix();
            });

            cancelBtn?.addEventListener("click", () => {
                inputClone.remove();
            });

            valuesContainer.appendChild(inputClone);
        });

        removeBtn.addEventListener("click", () => {
            clone.remove();
            rebuildMatrix();
        });

        builder.appendChild(clone);

        rebuildMatrix();
    }

    if (addBtn) {
        addBtn.addEventListener("click", () => {
            if (getAttributeCount() >= 3) return;
            createAttributeRow();
        });
    }

    updateAddButtonState();
});