let _allUnits = [];

const FormFields = {
    create: {
        itemName: "create_itemName",
        itemType: "create_itemType",
        sort: "create_sort",
        itemCode: "create_itemCode",
        subType: "create_subType",
        baseUnit: "create_baseUnit",
        displayUnit: "create_displayUnit",
        condition: "create_condition",
        error: "createErrorMsg"
    },
    edit: {
        itemName: "edit_itemName",
        itemType: "edit_itemType",
        sort: "edit_sort",
        itemCode: "edit_itemCode",
        subType: "edit_subType",
        baseUnit: "edit_baseUnit",
        displayUnit: "edit_displayUnit",
        condition: "edit_condition",
        error: "editErrorMsg"
    }
};

function resetForm(prefix) {
    const f = FormFields[prefix];

    document.getElementById(f.itemName).value = "";
    document.getElementById(f.sort).value = "0";
    document.getElementById(f.itemCode).value = "";
    document.getElementById(f.condition).value = "";

    const err = document.getElementById(f.error);
    if (err) err.classList.add("d-none");

    clearValidation(prefix);
}

function validateCreate(payload) {
    const f = FormFields.create;

    if (!payload.itemName?.trim()) {
        markInvalid(f.itemName);
        return "Item Name is required.";
    }

    if (!payload.itemTypeID) {
        markInvalid(f.itemType);
        return "Please select an Item Type.";
    }

    return null;
}

async function loadAllUnits() {
    if (_allUnits.length > 0) return;
    const res = await getJson("/Item/GetUnitsForItem?unitCategoryId=0");
    if (res.success) _allUnits = res.data;
}

function validateRequired(prefix, fieldName, message) {
    const id = `${prefix}_${fieldName}`;
    const el = document.getElementById(id);

    if (!el || !el.value || !el.value.toString().trim()) {
        markInvalid(id);
        return message;
    }

    return null;
}

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

async function openCreateModal() {
    resetForm("create");

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

    document.getElementById(
        "create_subType"
    ).innerHTML =
        '<option value="">-- Select Sub Type --</option>';

    document.getElementById("create_subType").disabled = true;

    await loadAllUnits();

    populateUnitDropdown("create_baseUnit", _allUnits);
    populateUnitDropdown("create_displayUnit", _allUnits);

    document.getElementById("create_displayUnit").disabled = false;

    openModal("createModal");

    clearValidation("create");
}

function initModalListeners() {

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

    document.getElementById("create_subType")
        .addEventListener("change", async function () {

            await refreshCreateCode();
        });

    document.getElementById("create_baseUnit")
        .addEventListener("change", async function () {

            const opt = this.options[this.selectedIndex];
            const categoryId = opt.dataset.categoryId;

            if (!categoryId) {

                await loadAllUnits();

                populateUnitDropdown("create_baseUnit", _allUnits, this.value);
                populateUnitDropdown("create_displayUnit", _allUnits);

                document.getElementById("create_displayUnit").disabled = false;

                return;
            }

            const res =await getJson(`/Item/GetUnitsForItem?unitCategoryId=${categoryId}`);

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

                document.getElementById(
                    "create_displayUnit"
                ).disabled = false;
            }
        });

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

    const payload = {
        itemName: document.getElementById("create_itemName").value.trim(),
        itemTypeID: parseInt(document.getElementById("create_itemType").value) || 0,
        sort: parseInt(document.getElementById("create_sort").value) || 0,
        itemCode: document.getElementById("create_itemCode").value.trim(),
        itemSubTypeID: parseInt(document.getElementById("create_subType").value) || null,
        baseUnitID: parseInt(document.getElementById("create_baseUnit").value) || null,
        displayUnitID: parseInt(document.getElementById("create_displayUnit").value) || null,
        condition: document.getElementById("create_condition").value
    };

    clearValidation("create");

    const error = validateCreate(payload);
    if (error) return showError("createErrorMsg", error);

    const res = await postJson("/Item/Create", payload);

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

    if (!res.success)
        return showToast(res.message, "error");

    const d = res.data;

    document.getElementById("edit_itemID").value = d.itemID;
    document.getElementById("edit_itemName").value = d.itemName ?? "";
    document.getElementById("edit_sort").value = d.sort ?? 0;
    document.getElementById("edit_itemCode").value = d.itemCode ?? "";
    document.getElementById("edit_condition").value = d.condition ?? "";
    document.getElementById("editErrorMsg").classList.add("d-none");

    const typeSelect = document.getElementById("edit_itemType");

    typeSelect.innerHTML =
        '<option value="">-- Select Item Type --</option>';

    d.itemTypeOptions.forEach(t => {

        const opt = document.createElement("option");

        opt.value = t.value;
        opt.textContent = t.text;

        if (t.value === d.itemTypeID)
            opt.selected = true;

        typeSelect.appendChild(opt);
    });

    const subSelect = document.getElementById("edit_subType");

    subSelect.innerHTML =
        '<option value="">-- Select Sub Type --</option>';

    if (d.subTypeOptions &&
        d.subTypeOptions.length > 0) {

        d.subTypeOptions.forEach(st => {

            const opt = document.createElement("option");

            opt.value = st.value;
            opt.textContent = st.text;

            if (st.value === d.itemSubTypeID)
                opt.selected = true;

            subSelect.appendChild(opt);
        });

        subSelect.disabled = false;
    }
    else {
        subSelect.disabled = true;
    }

    populateUnitDropdown(
        "edit_baseUnit",
        d.unitOptions,
        d.baseUnitID);

    populateUnitDropdown(
        "edit_displayUnit",
        d.unitOptions,
        d.displayUnitID);

    document.getElementById(
        "edit_displayUnit"
    ).disabled = !d.baseUnitID;

    openModal("editModal");

    clearValidation("edit");
}

// Save edited item
async function saveEdit() {

    const itemName = document.getElementById("edit_itemName").value.trim();
    const itemTypeID = parseInt(document.getElementById("edit_itemType").value) || 0;

    clearValidation("edit");

    if (!itemName) {
        markInvalid("edit_itemName");
        return showError("editErrorMsg", "Item Name is required.");
    }

    if (!itemTypeID) {
        markInvalid("edit_itemType");
        return showError("editErrorMsg", "Please select an Item Type.");
    }

    const payload = {
        itemID: parseInt(document.getElementById("edit_itemID").value),
        itemName,
        sort: parseInt(document.getElementById("edit_sort").value) || 0,
        itemCode: document.getElementById("edit_itemCode").value.trim(),
        itemTypeID,
        itemSubTypeID: parseInt(document.getElementById("edit_subType").value) || null,
        baseUnitID: parseInt(document.getElementById("edit_baseUnit").value) || null,
        displayUnitID: parseInt(document.getElementById("edit_displayUnit").value) || null,
        condition: document.getElementById("edit_condition").value
    };

    const res = await postJson("/Item/Update", payload);

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

    const modalBody = div.closest(".modal-body");
    if (modalBody) {
        modalBody.scrollTop = 0;
    }

    const modalDialog = div.closest(".modal");
    if (modalDialog) {
        modalDialog.scrollTop = 0;
    }

    div.setAttribute("tabindex", "-1");
    div.focus({ preventScroll: false });
}

function markInvalid(fieldId) {
    const el = document.getElementById(fieldId);
    if (el) el.classList.add("is-invalid");
}

function clearValidation(prefix) {
    const fields = [
        "itemName",
        "itemType",
        "itemCode",
        "sort",
        "condition"
    ];

    fields.forEach(f => {
        const el = document.getElementById(`${prefix}_${f}`);
        if (el) el.classList.remove("is-invalid");
    });

    document.getElementById(`${prefix}ErrorMsg`)
        ?.classList.add("d-none");
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