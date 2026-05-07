async function loadItems() {
    const params = new URLSearchParams({
        page: ItemState.currentPage,
        search: ItemState.search,
        sortColumn: ItemState.sortColumn,
        sortDirection: ItemState.sortDirection,
        itemTypeFilter: ItemState.itemTypeFilter,
        itemSubTypeFilter: ItemState.itemSubTypeFilter,
        conditionFilter: ItemState.conditionFilter
    });

    const res = await getJson(
        `/Item/GetItemsData?${params}`);

    if (!res.success) {
        showToast(res.message, "error");
        return;
    }

    renderTable(res.data);
    renderPagination(res.data);
}

function getConditionBadge(condition) {
    const map = {
        "New":
            '<span class="badge bg-success">New</span>',
        "Opened - Never Used":
            '<span class="badge bg-info text-dark">' +
            'Opened - Never Used</span>',
        "Used":
            '<span class="badge bg-warning text-dark">' +
            'Used</span>',
        "Defective":
            '<span class="badge bg-danger">' +
            'Defective</span>',
        "Disposed":
            '<span class="badge bg-secondary">' +
            'Disposed</span>',
        "Discontinued":
            '<span class="badge bg-dark">' +
            'Discontinued</span>'
    };
    return map[condition] ?? "—";
}

function renderTable(data) {
    const tbody =
        document.getElementById("itemTableBody");

    if (!data.items || data.items.length === 0) {
        tbody.innerHTML = `
      <tr>
        <td colspan="15"
            class="text-center text-muted py-4">
          No items found.
        </td>
      </tr>`;
        return;
    }

    tbody.innerHTML = data.items.map(item => `
    <tr data-id="${item.itemID}"
        data-has-base-unit="${item.baseUnitID ? 'true' : 'false'}"
        style="cursor:pointer;">
      <td onclick="event.stopPropagation()">
        <input type="checkbox"
               class="row-checkbox"
               value="${item.itemID}" />
      </td>
      <td data-col="col-2">
        ${item.itemCode ?? "—"}
      </td>
      <td data-col="col-3">${item.itemName}</td>
      <td data-col="col-4">${item.sort}</td>
      <td data-col="col-5">
        ${item.itemTypeName ?? "—"}
      </td>
      <td data-col="col-6">
        ${item.itemSubTypeName ?? "—"}
      </td>
      <td data-col="col-7">${item.variants}</td>
      <td data-col="col-8">
        ${item.baseUnitAbbreviation ?? "—"}
      </td>
      <td data-col="col-9">
        ${item.currentStock}
      </td>
      <td data-col="col-10">${item.unitCost}</td>
      <td data-col="col-11">
        ${getConditionBadge(item.condition)}
      </td>
      <td data-col="col-12">
        ${item.createdBy ?? "—"}
      </td>
      <td data-col="col-13">
        ${item.createdDate ?? "—"}
      </td>
      <td data-col="col-14">
        ${item.updatedBy ?? "—"}
      </td>
      <td data-col="col-15">
        ${item.updatedDate ?? "—"}
      </td>
    </tr>
  `).join("");

    initRowCheckboxes();
    applyColumnVisibility(
        JSON.parse(
            localStorage.getItem("item-visible-cols-v1")
        ) ?? getDefaultColumns()
    );
}

function getDefaultColumns() {
    return ["col-2", "col-3", "col-4", "col-5", "col-6",
        "col-7", "col-8", "col-9", "col-10", "col-11"];
}

function renderPagination(data) {
    document.getElementById("paginationInfo")
        .textContent =
        `Showing page ${data.pageNumber} ` +
        `of ${data.totalPages} ` +
        `(${data.totalCount} total records)`;

    const ul = document.getElementById(
        "paginationControls");
    ul.innerHTML = "";

    const prev = document.createElement("li");
    prev.className = `page-item ${!data.hasPreviousPage ? "disabled" : ""}`;
    prev.innerHTML =
        `<a class="page-link" href="#">Previous</a>`;
    prev.addEventListener("click", e => {
        e.preventDefault();
        if (data.hasPreviousPage) {
            ItemState.currentPage--;
            loadItems();
        }
    });
    ul.appendChild(prev);

    for (let i = 1; i <= data.totalPages; i++) {
        const li = document.createElement("li");
        li.className = `page-item ${i === data.pageNumber ? "active" : ""}`;
        li.innerHTML =
            `<a class="page-link" href="#">${i}</a>`;
        li.addEventListener("click", e => {
            e.preventDefault();
            ItemState.currentPage = i;
            loadItems();
        });
        ul.appendChild(li);
    }

    const next = document.createElement("li");
    next.className = `page-item ${!data.hasNextPage ? "disabled" : ""}`;
    next.innerHTML =
        `<a class="page-link" href="#">Next</a>`;
    next.addEventListener("click", e => {
        e.preventDefault();
        if (data.hasNextPage) {
            ItemState.currentPage++;
            loadItems();
        }
    });
    ul.appendChild(next);
}

function initRowCheckboxes() {
    ItemState.selectedIds = [];
    updateToolbarButtons();

    document.querySelectorAll(".row-checkbox")
        .forEach(cb => {
            cb.addEventListener("change", function () {
                const id = parseInt(this.value);
                const row = this.closest("tr");
                if (this.checked) {
                    if (!ItemState.selectedIds.includes(id))
                        ItemState.selectedIds.push(id);
                    row.classList.add("table-active");
                    ItemState.lastSelectedItemId = id;
                    ItemState.lastSelectedHasBaseUnit =
                        row.dataset.hasBaseUnit === "true";
                } else {
                    ItemState.selectedIds =
                        ItemState.selectedIds
                            .filter(x => x !== id);
                    row.classList.remove("table-active");
                }
                updateSelectAll();
                updateToolbarButtons();
            });
        });

    document.getElementById("selectAll")
        .addEventListener("change", function () {
            const checked = this.checked;
            document.querySelectorAll(".row-checkbox")
                .forEach(cb => {
                    cb.checked = checked;
                    const id = parseInt(cb.value);
                    const row = cb.closest("tr");
                    if (checked) {
                        if (!ItemState.selectedIds.includes(id))
                            ItemState.selectedIds.push(id);
                        row.classList.add("table-active");
                    } else {
                        row.classList.remove("table-active");
                    }
                });
            if (!checked) ItemState.selectedIds = [];
            updateToolbarButtons();
        });
}

function updateSelectAll() {
    const all =
        document.querySelectorAll(".row-checkbox");
    const checked =
        document.querySelectorAll(
            ".row-checkbox:checked");
    const sa =
        document.getElementById("selectAll");
    sa.checked =
        all.length > 0 &&
        checked.length === all.length;
    sa.indeterminate =
        checked.length > 0 &&
        checked.length < all.length;
}

function updateToolbarButtons() {
    const count = ItemState.selectedIds.length;
    const btnEdit =
        document.getElementById("btnEdit");
    const btnDelete =
        document.getElementById("btnDelete");
    const btnConversions =
        document.getElementById("btnConversions");
    const selectedCount =
        document.getElementById("selectedCount");

    btnEdit.disabled = count !== 1;
    btnDelete.disabled = count === 0;

    if (count === 1 &&
        ItemState.lastSelectedHasBaseUnit) {
        btnConversions.style.display = "";
    } else {
        btnConversions.style.display = "none";
    }

    selectedCount.textContent =
        count === 0 ? "" :
            count === 1 ? "1 item selected" :
                `${count} items selected`;
}