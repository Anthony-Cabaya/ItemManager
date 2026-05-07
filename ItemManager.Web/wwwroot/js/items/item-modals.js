let _allUnits = [];

// Load all units from server
async function loadAllUnits() {
    if (_allUnits.length > 0) return;
    const res = await getJson("/Item/GetUnitsForItem?unitCategoryId=0");
    if (res.success) _allUnits = res.data;
}

// Populate a select element with units
function populateUnitDropdown(selectId, units, selectedValue = "") {
    const sel = document.getElementById(selectId);
    sel.innerHTML = '<option value="">-- No Unit --</option>';
    units.forEach(u => {
        const opt = document.createElement("option");
        opt.value = u.value;
        opt.textContent = u.text;
        opt.dataset.categoryId = u.categoryId;
        if (String(u.value) === String(selectedValue)) opt.selected = true;
        sel.appendChild(opt);
    });
}

// CREATE MODAL FUNCTIONS
async function openCreateModal() {
    document.getElementById("create_itemName").value = "";
    document.getElementById("create_sort").value = "0";
    document.getElementById("create_itemCode").value = "";
    document.getElementById("create_condition").value = "";
    document.getElementById("createErrorMsg").classList.add("d-none");

    const typeSelect = document.getElementById("create_itemType");
    typeSelect.innerHTML = '<option value="">-- Select Item Type --</option>';

    const pageData = document.getElementById("pageData");
    const itemTypes = JSON.parse(pageData.dataset.itemTypes);
    itemTypes.forEach(t => {
        const opt = document.createElement("option");
        opt.value = t.value;
        opt.textContent = t.text;
        typeSelect.appendChild(opt);
    });

    document.getElementById("create_subType").innerHTML = '<option value="">-- Select Sub Type --</option>';
    document.getElementById("create_subType").disabled = true;

    await loadAllUnits();
    populateUnitDropdown("create_baseUnit", _allUnits);
    populateUnitDropdown("create_displayUnit", _allUnits);

    openModal("createModal");
}

// Handle change of ItemType in create modal
function initModalListeners() {

    // CREATE ITEM TYPE
    document.getElementById("create_itemType")
        .addEventListener("change", async function () {

            const typeId = parseInt(this.value) || 0;

            const subSel =
                document.getElementById("create_subType");

            subSel.innerHTML =
                '<option value="">-- Select Sub Type --</option>';

            subSel.disabled = true;

            if (typeId > 0) {

                const res =
                    await getJson(
                        `/Item/GetSubTypesByItemType?itemTypeId=${typeId}`);

                if (res && res.length > 0) {

                    res.forEach(st => {

                        const opt =
                            document.createElement("option");

                        opt.value = st.value;
                        opt.textContent = st.text;

                        subSel.appendChild(opt);
                    });

                    subSel.disabled = false;
                }

                await refreshCreateCode();
            }
        });

    // CREATE SUBTYPE
    document.getElementById("create_subType")
        .addEventListener("change", async function () {

            await refreshCreateCode();
        });

    // CREATE BASE UNIT
    document.getElementById("create_baseUnit")
        .addEventListener("change", async function () {

            const opt =
                this.options[this.selectedIndex];

            const categoryId =
                opt.dataset.categoryId;

            if (!categoryId) {

                await loadAllUnits();

                populateUnitDropdown(
                    "create_baseUnit",
                    _allUnits,
                    this.value);

                populateUnitDropdown(
                    "create_displayUnit",
                    _allUnits);

                return;
            }

            const res =
                await getJson(
                    `/Item/GetUnitsForItem?unitCategoryId=${categoryId}`);

            if (res.success) {

                const currentBase = this.value;

                populateUnitDropdown(
                    "create_baseUnit",
                    res.data,
                    currentBase);

                populateUnitDropdown(
                    "create_displayUnit",
                    res.data,
                    currentBase);
            }
        });

    // EDIT BASE UNIT
    document.getElementById("edit_baseUnit")
        .addEventListener("change", async function () {

            const opt =
                this.options[this.selectedIndex];

            const categoryId =
                opt.dataset.categoryId;

            if (!categoryId)
                return;

            const res =
                await getJson(
                    `/Item/GetUnitsForItem?unitCategoryId=${categoryId}`);

            if (res.success) {

                const currentBase = this.value;

                populateUnitDropdown(
                    "edit_baseUnit",
                    res.data,
                    currentBase);

                populateUnitDropdown(
                    "edit_displayUnit",
                    res.data,
                    currentBase);
            }
        });
}

// Refresh the auto-generated Item Code in create modal
async function refreshCreateCode() {
    const typeId = parseInt(document.getElementById("create_itemType").value) || 0;
    const subId = parseInt(document.getElementById("create_subType").value) || null;

    if (typeId === 0) return;

    const params = new URLSearchParams({ itemTypeId: typeId });
    if (subId) params.append("itemSubTypeId", subId);

    const res = await getJson(`/Item/GetGeneratedCode?${params}`);
    if (res.success) {
        document.getElementById("create_itemCode").value = res.data.code;
    }
}

// Save new item
async function saveCreate() {
    const itemName = document.getElementById("create_itemName").value.trim();
    const sort = parseInt(document.getElementById("create_sort").value) || 0;
    const itemCode = document.getElementById("create_itemCode").value.trim();
    const itemTypeID = parseInt(document.getElementById("create_itemType").value) || 0;
    const itemSubTypeID = parseInt(document.getElementById("create_subType").value) || null;
    const baseUnitID = parseInt(document.getElementById("create_baseUnit").value) || null;
    const displayUnitID = parseInt(document.getElementById("create_displayUnit").value) || null;
    const condition = document.getElementById("create_condition").value;

    if (!itemName) return showError("createErrorMsg", "Item Name is required.");
    if (!itemTypeID) return showError("createErrorMsg", "Please select an Item Type.");

    const res = await postJson("/Item/Create", { itemName, sort, itemCode, itemTypeID, itemSubTypeID, baseUnitID, displayUnitID, condition });
    if (res.success) {
        closeModal("createModal");
        showToast(res.message, "create");
        loadItems();
    } else {
        showError("createErrorMsg", res.message);
    }
}

// EDIT MODAL FUNCTIONS
async function openEditModal(itemId) {
    const res = await getJson(`/Item/GetItemForEdit?id=${itemId}`);
    if (!res.success) return showToast(res.message, "error");

    const d = res.data;

    document.getElementById("edit_itemID").value = d.itemID;
    document.getElementById("edit_itemName").value = d.itemName ?? "";
    document.getElementById("edit_sort").value = d.sort ?? 0;
    document.getElementById("edit_itemCode").value = d.itemCode ?? "";
    document.getElementById("edit_condition").value = d.condition ?? "";
    document.getElementById("editErrorMsg").classList.add("d-none");

    const typeSelect = document.getElementById("edit_itemType");
    typeSelect.innerHTML = '<option value="">-- Select Item Type --</option>';
    d.itemTypeOptions.forEach(t => {
        const opt = document.createElement("option");
        opt.value = t.value;
        opt.textContent = t.text;
        if (t.value === d.itemTypeID) opt.selected = true;
        typeSelect.appendChild(opt);
    });

    const subSelect = document.getElementById("edit_subType");
    subSelect.innerHTML = '<option value="">-- Select Sub Type --</option>';
    if (d.subTypeOptions && d.subTypeOptions.length > 0) {
        d.subTypeOptions.forEach(st => {
            const opt = document.createElement("option");
            opt.value = st.value;
            opt.textContent = st.text;
            if (st.value === d.itemSubTypeID) opt.selected = true;
            subSelect.appendChild(opt);
        });
        subSelect.disabled = false;
    } else subSelect.disabled = true;

    populateUnitDropdown("edit_baseUnit", d.unitOptions, d.baseUnitID);
    populateUnitDropdown("edit_displayUnit", d.unitOptions, d.displayUnitID);

    openModal("editModal");
}

// Save edited item
async function saveEdit() {
    const itemID = parseInt(document.getElementById("edit_itemID").value);
    const itemName = document.getElementById("edit_itemName").value.trim();
    const sort = parseInt(document.getElementById("edit_sort").value) || 0;
    const itemCode = document.getElementById("edit_itemCode").value.trim();
    const itemTypeID = parseInt(document.getElementById("edit_itemType").value) || 0;
    const itemSubTypeID = parseInt(document.getElementById("edit_subType").value) || null;
    const baseUnitID = parseInt(document.getElementById("edit_baseUnit").value) || null;
    const displayUnitID = parseInt(document.getElementById("edit_displayUnit").value) || null;
    const condition = document.getElementById("edit_condition").value;

    if (!itemName) return showError("editErrorMsg", "Item Name is required.");
    if (!itemTypeID) return showError("editErrorMsg", "Please select an Item Type.");

    const res = await postJson("/Item/Update", {
        itemID,
        itemName,
        sort,
        itemCode,
        itemTypeID,
        itemSubTypeID,
        baseUnitID,
        displayUnitID,
        condition
    });

    if (res.success) {
        closeModal("editModal");
        showToast(res.message, "edit");
        loadItems();
    } else {
        showError("editErrorMsg", res.message);
    }
}

// DELETE MODAL FUNCTIONS
function openDeleteModal() {
    document.getElementById("deleteCount").textContent = ItemState.selectedIds.length;
    openModal("deleteModal");
}

async function deleteItems() {
    const res = await postJson("/Item/DeleteItems", { ids: ItemState.selectedIds });

    if (res.success) {
        closeModal("deleteModal");
        ItemState.selectedIds = [];
        showToast(res.message, "delete");
        loadItems();
    } else {
        showToast(res.message, "error");
    }
}

// HELPER FUNCTIONS
function showError(divId, message) {
    const div = document.getElementById(divId);
    div.textContent = message;
    div.classList.remove("d-none");
}

window.ItemModals = {
    openCreateModal,
    saveCreate,
    openEditModal,
    saveEdit,
    openDeleteModal,
    deleteItems,
    initModalListeners
};