async function loadItemTypes() {

    const params = new URLSearchParams({
        page: ItemTypeState.currentPage,
        search: ItemTypeState.search
    });

    const res = await getJson(
        `/ItemType/GetItemTypesData?${params}`);

    if (!res.success) {
        showToast(res.message, "error");
        return;
    }

    renderItemTypeTable(res.data);
    renderItemTypePagination(res.data);
}

function renderItemTypeTable(data) {

    const tbody =
        document.getElementById("itemTypeTableBody");

    if (!data.items || data.items.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="7"
                    class="text-center text-muted py-4">
                    No item types found.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = data.items.map(item => `
        <tr data-id="${item.itemTypeID}"
            style="cursor:pointer;">

            <td onclick="event.stopPropagation()">

                <input type="checkbox"
                       class="row-checkbox"
                       value="${item.itemTypeID}" />

            </td>

            <td>${item.itemTypeName ?? "—"}</td>

            <td>${item.sort}</td>

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

    document
        .querySelectorAll(".audit-col")
        .forEach(col => {

            if (!ItemTypeState.isAdmin) {
                col.classList.add("d-none");
            }
            else {
                col.classList.remove("d-none");
            }
        });
}

function initRowCheckboxes() {

    const tableBody = document.getElementById("itemTypeTableBody");
    const selectAll = document.getElementById("selectAll");

    if (!tableBody.dataset.bound) {
        tableBody.dataset.bound = "true";

        tableBody.addEventListener("change", function (e) {

            if (!e.target.classList.contains("row-checkbox"))
                return;

            const cb = e.target;
            const id = Number(cb.value);

            if (cb.checked) {

                if (!ItemTypeState.selectedIds.includes(id)) {
                    ItemTypeState.selectedIds.push(id);
                }

                cb.closest("tr")?.classList.add("table-active");
            }
            else {

                ItemTypeState.selectedIds =
                    ItemTypeState.selectedIds.filter(x => x !== id);

                cb.closest("tr")?.classList.remove("table-active");
            }

            updateToolbarButtons();
            updateSelectAll();
        });
    }

    if (!selectAll.dataset.bound) {
        selectAll.dataset.bound = "true";

        selectAll.addEventListener("change", function () {

            const checkboxes =
                document.querySelectorAll(".row-checkbox");

            if (this.checked) {

                ItemTypeState.selectedIds = [];

                checkboxes.forEach(cb => {
                    cb.checked = true;
                    cb.closest("tr")?.classList.add("table-active");
                    ItemTypeState.selectedIds.push(Number(cb.value));
                });

            } else {

                checkboxes.forEach(cb => {
                    cb.checked = false;
                    cb.closest("tr")?.classList.remove("table-active");
                });

                ItemTypeState.selectedIds = [];
            }

            updateToolbarButtons();
            updateSelectAll();
        });
    }

    if (selectAll) {
        selectAll.checked = false;
        selectAll.indeterminate = false;
    }

    updateToolbarButtons();
}

function updateSelectAll() {

    const selectAll = document.getElementById("selectAll");
    if (!selectAll) return;

    const checkboxes = document.querySelectorAll(".row-checkbox");
    const checked = document.querySelectorAll(".row-checkbox:checked");

    selectAll.checked =
        checkboxes.length > 0 &&
        checked.length === checkboxes.length;

    selectAll.indeterminate =
        checked.length > 0 &&
        checked.length < checkboxes.length;
}

function updateToolbarButtons() {

    const count = ItemTypeState.selectedIds.length;

    document.getElementById("btnEdit")
        .disabled = count !== 1;

    document.getElementById("btnDelete")
        .disabled = count === 0;

    const selectedCount =
        document.getElementById("selectedCount");

    if (count === 0) {
        selectedCount.textContent = "";
    }
    else if (count === 1) {
        selectedCount.textContent =
            "1 item type selected";
    }
    else {
        selectedCount.textContent =
            `${count} item types selected`;
    }
}

function renderItemTypePagination(data) {

    const pagination =
        document.getElementById("paginationControls");

    const info =
        document.getElementById("paginationInfo");

    let html = "";

    html += `
        <li class="page-item ${!data.hasPreviousPage ? "disabled" : ""}">
            <button class="page-link"
                    onclick="changePage(${data.pageNumber - 1})">
                Previous
            </button>
        </li>
    `;

    for (let i = 1; i <= data.totalPages; i++) {

        html += `
            <li class="page-item ${i === data.pageNumber ? "active" : ""}">
                <button class="page-link"
                        onclick="changePage(${i})">
                    ${i}
                </button>
            </li>
        `;
    }

    html += `
        <li class="page-item ${!data.hasNextPage ? "disabled" : ""}">
            <button class="page-link"
                    onclick="changePage(${data.pageNumber + 1})">
                Next
            </button>
        </li>
    `;

    pagination.innerHTML = html;

    info.textContent =
        `Page ${data.pageNumber} of ${data.totalPages}
         (${data.totalCount} total item types)`;
}

async function changePage(page) {

    if (page < 1)
        return;

    ItemTypeState.currentPage = page;

    ItemTypeState.selectedIds = [];

    await loadItemTypes();
}