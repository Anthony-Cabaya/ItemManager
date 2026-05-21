window.VariantMatrix = {

    rebuild() {

        const page = document.getElementById("page-item-code");
        const parentCode = page?.value ?? "";

        const container = document.getElementById("matrix-table-container");
        const section = document.getElementById("matrix-section");

        if (!container || !section) return;

        const attributes = this.collectAttributes();

        if (
            attributes.length === 0 ||
            attributes.some(a => a.values.length === 0)
        ) {
            container.innerHTML = "";
            section.style.display = "none";
            return;
        }

        const combinations = this.cartesian(
            attributes.map(a => a.values)
        );

        const table = this.renderTable(combinations, parentCode);

        container.innerHTML = "";
        container.appendChild(table);

        section.style.display = "block";
    },

    collectAttributes() {

        const cards = document.querySelectorAll("#attribute-builder .attribute-card");

        return Array.from(cards).map(card => {

            const name = card.querySelector(".attribute-name")?.value || "";

            const values = Array.from(
                card.querySelectorAll(".values-container .badge")
            ).map(b => ({
                label: b.dataset.valueLabel,
                id: b.dataset.valueId || ""
            }));

            return { name, values };
        });
    },

    cartesian(arrays) {
        return arrays.reduce((acc, curr) => {
            const result = [];

            acc.forEach(a => {
                curr.forEach(b => {
                    result.push(a.concat([b]));
                });
            });

            return result;
        }, [[]]);
    },

    renderTable(combinations, parentCode) {

        const table = document.createElement("table");
        table.className = "table table-hover table-sm align-middle border-bottom";
        table.style.fontSize = "13px";

        const thead = document.createElement("thead");
        thead.className = "table-light";

        thead.innerHTML = `
            <tr>
                <th style="width:45px;" class="text-center">
                    <input type="checkbox" id="matrix-select-all" checked />
                </th>
                <th style="width:260px;">VARIANT CODE</th>
                <th>VARIANT NAME</th>
            </tr>
        `;

        const tbody = document.createElement("tbody");

        combinations.forEach(combo => {

            const suffix = combo
                .map(x => x.label.trim()
                    .toUpperCase()
                    .replace(/\s+/g, '-'))
                .join('-');

            const generatedCode =
                (parentCode ? parentCode + "-" : "") + suffix;

            const variantName = combo
                .map(x => x.label)
                .join(" / ");

            const ids = combo.map(x => x.id || 0);

            const row = document.createElement("tr");

            row.dataset.valueIds = JSON.stringify(ids);
            row.dataset.variantName = variantName;

            row.innerHTML = `
                <td class="text-center">
                    <input type="checkbox" class="matrix-check" checked />
                </td>

                <td>
                    <input type="text"
                           class="form-control form-control-sm font-monospace variant-code-input"
                           value="${generatedCode.toUpperCase()}"
                           style="color:#6c757d;
                                  background:transparent;
                                  border:none;
                                  border-bottom:1px solid #dee2e6;
                                  border-radius:0;
                                  padding-left:0;" />
                </td>

                <td class="variant-name-cell">
                    ${variantName}
                </td>
            `;

            const input = row.querySelector(".variant-code-input");

            input.addEventListener("focus", () => {
                input.style.borderBottom = "2px solid #1e293b";
                input.style.color = "#000";
            });

            input.addEventListener("blur", () => {
                input.style.borderBottom = "1px solid #dee2e6";
                if (input.value === input.defaultValue) {
                    input.style.color = "#6c757d";
                }
            });

            tbody.appendChild(row);
        });

        table.appendChild(thead);
        table.appendChild(tbody);

        return table;
    },

    getCheckedRows() {

        const rows = document.querySelectorAll("#matrix-table-container tbody tr");

        const result = [];

        rows.forEach(row => {

            const checkbox = row.querySelector(".matrix-check");
            const codeInput = row.querySelector(".variant-code-input");

            if (!checkbox?.checked) return;

            result.push({
                variantCode: codeInput?.value || "",
                variantName: row.dataset.variantName,
                attributeValueIds: JSON.parse(row.dataset.valueIds || "[]"),
                isChecked: true
            });
        });

        return result;
    },

    async saveAndRebuild(itemId) {

        const attributes = this.collectAttributes();

        if (attributes.length === 0) return;

        const payload = {
            itemID: itemId,
            attributes: attributes.map(a => ({
                attributeName: a.name,
                values: a.values.map(v => v.label)
            }))
        };

        const res = await fetch("/ItemVariant/SaveAttributes", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!data.success) return;

        const savedAttrs = data.data;

        const cards = document.querySelectorAll("#attribute-builder .attribute-card");

        cards.forEach((card, ai) => {

            const savedAttr = savedAttrs[ai];
            if (!savedAttr) return;

            const badges = card.querySelectorAll(".values-container .badge");

            badges.forEach((badge, vi) => {
                const savedVal = savedAttr.values[vi];
                if (savedVal) {
                    badge.dataset.valueId = savedVal.id.toString();
                }
            });
        });

        this.rebuild();
    }

};