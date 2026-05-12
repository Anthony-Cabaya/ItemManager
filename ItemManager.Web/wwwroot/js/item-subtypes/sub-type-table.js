async function loadSubTypes() {

    const params = new URLSearchParams({
        page: SubTypeState.currentPage,
        search: SubTypeState.search,
        itemTypeFilter: SubTypeState.itemTypeFilter
    });

    try {

        const res = await getJson(
            `/ItemSubType/GetSubTypesData?${params}`);

        if (!res.success) {
            showToast(res.message || "Failed to load sub types.", "error");
            return;
        }

        renderSubTypeTable(res.data);
        renderSubTypePagination(res.data);

    } catch (err) {

        console.error(err);
        showToast("An error occurred while loading sub types.", "error");
    }
}

function renderSubTypeTable(data) {

    const tbody = document.getElementById("subTypeTableBody");

    if (!tbody) {
        return;
    }

    const items = data.items || data.data || [];

    if (items.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="8"
                    class="text-center text-muted py-4">
                    No sub types found.
                </td>
            </tr>
        `;

        updateToolbarButtons();
        return;
    }

    tbody.innerHTML = items.map(item => `
        <tr data-id="${item.itemSubTypeID}"
            style="cursor:pointer;">
            
            <td onclick="event.stopPropagation()">
                <input type="checkbox"
                       class="row-checkbox"
                       value="${item.itemSubTypeID}" />
            </td>

            <td>${item.itemSubTypeName ?? "—"}</td>
            <td>${item.itemTypeName ?? "—"}</td>
            <td>${item.sort ?? 0}</td>

            <td class="audit-col">
                ${item.createdBy ?? "—"}
            </td>

            <td class="audit-col">
                ${item.createdDate ?? "—"}
            </td>

            <td class="audit-col">
                ${item.updatedBy ?? "—"}
            </td>

            <td class="audit-col">
                ${item.updatedDate ?? "—"}
            </td>
        </tr>
    `).join("");

    initRowCheckboxes();
    applyAuditVisibility();
}

function applyAuditVisibility() {

    const auditColumns = document
        .querySelectorAll(".audit-col");

    auditColumns.forEach(col => {

        if (!SubTypeState.isAdmin) {
            col.classList.add("d-none");
        } else {
            col.classList.remove("d-none");
        }
    });
}

function initRowCheckboxes() {

    const tbody = document.getElementById("subTypeTableBody");
    const selectAll = document.getElementById("selectAll");

    if (!tbody) {
        return;
    }

    if (selectAll) {
        selectAll.checked = false;
        selectAll.indeterminate = false;
    }

    if (!tbody.dataset.bound) {

        tbody.addEventListener("change", function (e) {

            if (!e.target.classList.contains("row-checkbox")) {
                return;
            }

            const checkbox = e.target;
            const id = parseInt(checkbox.value);

            if (checkbox.checked) {

                if (!SubTypeState.selectedIds.includes(id)) {
                    SubTypeState.selectedIds.push(id);
                }

            } else {

                SubTypeState.selectedIds =
                    SubTypeState.selectedIds
                        .filter(x => x !== id);
            }

            updateSelectAll();
            updateToolbarButtons();
        });

        tbody.dataset.bound = "true";
    }

    if (selectAll && !selectAll.dataset.bound) {

        selectAll.addEventListener("change", function () {

            const checkboxes =
                tbody.querySelectorAll(".row-checkbox");

            SubTypeState.selectedIds = [];

            checkboxes.forEach(cb => {

                cb.checked = selectAll.checked;

                if (selectAll.checked) {
                    SubTypeState.selectedIds.push(
                        parseInt(cb.value));
                }
            });

            updateToolbarButtons();
            updateSelectAll();
        });

        selectAll.dataset.bound = "true";
    }

    updateToolbarButtons();
}

function updateSelectAll() {

    const selectAll = document.getElementById("selectAll");

    if (!selectAll) {
        return;
    }

    const checkboxes = document
        .querySelectorAll("#subTypeTableBody .row-checkbox");

    const checked = document
        .querySelectorAll(
            "#subTypeTableBody .row-checkbox:checked");

    if (checkboxes.length === 0) {

        selectAll.checked = false;
        selectAll.indeterminate = false;
        return;
    }

    if (checked.length === 0) {

        selectAll.checked = false;
        selectAll.indeterminate = false;

    } else if (checked.length === checkboxes.length) {

        selectAll.checked = true;
        selectAll.indeterminate = false;

    } else {

        selectAll.checked = false;
        selectAll.indeterminate = true;
    }
}

function updateToolbarButtons() {

    const count = SubTypeState.selectedIds.length;

    const btnEdit = document.getElementById("btnEdit");
    const btnDelete = document.getElementById("btnDelete");
    const selectedCount =
        document.getElementById("selectedCount");

    if (btnEdit) {
        btnEdit.disabled = count !== 1;
    }

    if (btnDelete) {
        btnDelete.disabled = count === 0;
    }

    if (!selectedCount) {
        return;
    }

    if (count === 0) {

        selectedCount.textContent = "";

    } else if (count === 1) {

        selectedCount.textContent =
            "1 sub type selected";

    } else {

        selectedCount.textContent =
            `${count} sub types selected`;
    }
}

function renderSubTypePagination(data) {

    const pagination =
        document.getElementById("paginationControls");

    const info =
        document.getElementById("paginationInfo");

    if (!pagination || !info) {
        return;
    }

    const currentPage = data.pageNumber || 1;
    const totalPages = data.totalPages || 1;
    const totalCount = data.totalCount || 0;

    info.textContent =
        `Page ${currentPage} of ${totalPages} (${totalCount} total sub types)`;

    if (totalPages <= 1) {

        pagination.innerHTML = "";
        return;
    }

    let html = "";

    html += `
        <li class="page-item ${currentPage === 1
            ? "disabled"
            : ""
        }">
            <button class="page-link"
                    onclick="changePage(${currentPage - 1})">
                Previous
            </button>
        </li>
    `;

    for (let i = 1; i <= totalPages; i++) {

        html += `
            <li class="page-item ${i === currentPage
                ? "active"
                : ""
            }">
                <button class="page-link"
                        onclick="changePage(${i})">
                    ${i}
                </button>
            </li>
        `;
    }

    html += `
        <li class="page-item ${currentPage === totalPages
            ? "disabled"
            : ""
        }">
            <button class="page-link"
                    onclick="changePage(${currentPage + 1})">
                Next
            </button>
        </li>
    `;

    pagination.innerHTML = html;
}

async function changePage(page) {

    if (page < 1) {
        return;
    }

    SubTypeState.currentPage = page;
    SubTypeState.selectedIds = [];

    await loadSubTypes();
}