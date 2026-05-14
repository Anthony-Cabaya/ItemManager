async function loadCategories() {

    const params = new URLSearchParams({
        page: CategoryState.currentPage,
        search: CategoryState.search
    });

    const res = await getJson(
        `/UnitCategory/GetCategoriesData?${params}`
    );

    if (!res.success) {
        showToast(res.message, "error");
        return;
    }

    CategoryState.totalPages =
        res.data.totalPages;

    renderCategoryTable(res.data);
    renderCategoryPagination(res.data);
}

function renderCategoryTable(data) {

    const tbody = document.getElementById("categoryTableBody");

    if (!data.items || data.items.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="8"
                    class="text-center text-muted py-4">
                    No categories found.
                </td>
            </tr>
        `;

        updateToolbarButtons();
        return;
    }

    tbody.innerHTML = data.items.map(x => `
        <tr data-id="${x.unitCategoryID}"
            data-is-system="${x.isSystem}"
            style="cursor:pointer;">

            <td onclick="event.stopPropagation()">
                <input type="checkbox"
                       class="row-checkbox"
                       value="${x.unitCategoryID}" />
            </td>

            <td>${x.categoryName}</td>

            <td>
                ${x.isSystem
            ? `<span class="badge bg-primary">
                           System
                       </span>`
            : "—"}
            </td>

            <td>${x.sort}</td>

            <td class="audit-col">
                ${x.createdBy ?? "—"}
            </td>

            <td class="audit-col">
                ${x.createdDate ?? "—"}
            </td>

            <td class="audit-col">
                ${x.updatedBy ?? "—"}
            </td>

            <td class="audit-col">
                ${x.updatedDate ?? "—"}
            </td>

        </tr>
    `).join("");

    initRowCheckboxes();
    applyAuditVisibility();
}

function renderCategoryPagination(data) {

    const pagination = document.getElementById("paginationControls");

    const info = document.getElementById("paginationInfo");

    pagination.innerHTML = "";

    info.textContent =
        `Page ${data.pageNumber} of ${data.totalPages}
         (${data.totalCount} total categories)`;

    if (data.totalPages <= 1)
        return;

    let html = `
        <li class="page-item
            ${!data.hasPreviousPage
            ? "disabled"
            : ""}">

            <button class="page-link"
                    onclick="changePage(${data.pageNumber - 1})"
                    ${!data.hasPreviousPage
            ? "disabled"
            : ""}>
                Previous
            </button>

        </li>
    `;

    for (let i = 1; i <= data.totalPages; i++) {

        html += `
            <li class="page-item
                ${i === data.pageNumber
                ? "active"
                : ""}">

                <button class="page-link"
                        onclick="changePage(${i})">
                    ${i}
                </button>

            </li>
        `;
    }

    html += `
        <li class="page-item
            ${!data.hasNextPage
            ? "disabled"
            : ""}">

            <button class="page-link"
                    onclick="changePage(${data.pageNumber + 1})"
                    ${!data.hasNextPage
            ? "disabled"
            : ""}>
                Next
            </button>

        </li>
    `;

    pagination.innerHTML = html;
}

function applyAuditVisibility() {

    const auditCols = document.querySelectorAll(".audit-col");

    auditCols.forEach(col => {

        col.style.display =
            CategoryState.isAdmin
                ? ""
                : "none";
    });
}

function initRowCheckboxes() {

    bindRowCheckboxEvents();
    bindSelectAllEvent();
}

function bindRowCheckboxEvents() {

    const tbody = document.getElementById("categoryTableBody");

    if (tbody.dataset.bound)
        return;

    tbody.addEventListener("change", e => {

        if (!e.target.classList.contains(
            "row-checkbox")) {
            return;
        }

        const checkbox = e.target;
        const row = checkbox.closest("tr");
        const id = parseInt(checkbox.value);

        if (checkbox.checked) {

            if (!CategoryState.selectedIds
                .includes(id)) {

                CategoryState.selectedIds.push(id);
            }

        } else {

            CategoryState.selectedIds =
                CategoryState.selectedIds
                    .filter(x => x !== id);
        }

        updateSelectedSystemState(row);
        updateSelectAll();
        updateToolbarButtons();
    });

    tbody.dataset.bound = "true";
}

function bindSelectAllEvent() {

    const selectAll = document.getElementById("selectAll");

    if (selectAll.dataset.bound)
        return;

    selectAll.addEventListener("change", () => {

        const checked = selectAll.checked;
        const checkboxes = document.querySelectorAll(".row-checkbox");

        CategoryState.selectedIds = [];

        checkboxes.forEach(cb => {

            cb.checked = checked;

            if (checked) {

                CategoryState.selectedIds.push(
                    parseInt(cb.value)
                );
            }
        });

        const selectedRow =
            document.querySelector(".row-checkbox:checked")
                ?.closest("tr");

        updateSelectedSystemState(selectedRow);
        updateToolbarButtons();
    });

    selectAll.dataset.bound = "true";
}

function updateSelectedSystemState(row) {

    if (CategoryState.selectedIds.length === 1) {

        CategoryState.lastSelectedIsSystem =
            row?.dataset.isSystem === "true";

    } else {

        CategoryState.lastSelectedIsSystem =
            false;
    }
}

function updateSelectAll() {

    const selectAll = document.getElementById("selectAll");
    const checkboxes = document.querySelectorAll(".row-checkbox");

    if (checkboxes.length === 0) {

        selectAll.checked = false;
        return;
    }

    selectAll.checked =
        Array.from(checkboxes)
            .every(x => x.checked);
}

function updateToolbarButtons() {

    const count = CategoryState.selectedIds.length;
    const btnEdit = document.getElementById("btnEdit");
    const btnDelete = document.getElementById("btnDelete");
    const selectedCount = document.getElementById("selectedCount");

    btnEdit.disabled = count !== 1;

    btnDelete.disabled =
        count === 0 ||
        (count === 1 &&
            CategoryState.lastSelectedIsSystem);

    if (count === 0) {

        selectedCount.textContent = "";

    } else if (count === 1) {

        selectedCount.textContent = "1 category selected";

    } else {

        selectedCount.textContent = `${count} categories selected`;
    }
}

async function changePage(page) {

    if (page < 1 || page > CategoryState.totalPages) {
        return;
    }

    CategoryState.currentPage = page;
    CategoryState.selectedIds = [];

    await loadCategories();
}