function showError(divId, message) {

    const div = document.getElementById(divId);

    if (!div) {
        return;
    }

    div.textContent = message;
    div.classList.remove("d-none");

    div.setAttribute("tabindex", "-1");
    div.focus();

    const modal = div.closest(".modal");

    if (modal) {
        modal.scrollTop = 0;
    }
}

function markInvalid(fieldId) {

    const field =
        document.getElementById(fieldId);

    if (field) {
        field.classList.add("is-invalid");
    }
}

function clearValidation(prefix) {

    [
        `${prefix}_subTypeName`,
        `${prefix}_itemType`,
        `${prefix}_sort`
    ].forEach(id => {

        const field =
            document.getElementById(id);

        if (field) {
            field.classList.remove("is-invalid");
        }
    });

    const errorDiv =
        document.getElementById(
            `${prefix}ErrorMsg`);

    if (errorDiv) {

        errorDiv.textContent = "";
        errorDiv.classList.add("d-none");
    }
}

function populateItemTypeDropdown(
    selectId,
    selectedValue = "") {

    const pageData =
        document.getElementById("pageData");

    const itemTypes = JSON.parse(
        pageData.dataset.itemTypes || "[]");

    const select =
        document.getElementById(selectId);

    if (!select) {
        return;
    }

    select.innerHTML = "";

    if (selectId === "create_itemType") {

        const defaultOpt =
            document.createElement("option");

        defaultOpt.value = "";
        defaultOpt.textContent =
            "-- Select Item Type --";

        select.appendChild(defaultOpt);
    }

    itemTypes.forEach(t => {

        const opt =
            document.createElement("option");

        opt.value = t.value;
        opt.textContent = t.text;

        if (selectedValue &&
            parseInt(selectedValue) ===
            parseInt(t.value)) {

            opt.selected = true;
        }

        select.appendChild(opt);
    });
}

async function openCreateModal() {

    document.getElementById(
        "create_subTypeName").value = "";

    document.getElementById(
        "create_sort").value = "0";

    populateItemTypeDropdown(
        "create_itemType");

    clearValidation("create");

    openModal("createModal");
}

async function saveCreate() {

    clearValidation("create");

    const subTypeName =
        document.getElementById(
            "create_subTypeName")
            .value
            .trim();

    const itemTypeID = parseInt(
        document.getElementById(
            "create_itemType").value) || 0;

    const sort = parseInt(
        document.getElementById(
            "create_sort").value) || 0;

    if (!subTypeName) {

        markInvalid("create_subTypeName");

        showError(
            "createErrorMsg",
            "Sub Type Name is required.");

        return;
    }

    if (itemTypeID === 0) {

        markInvalid("create_itemType");

        showError(
            "createErrorMsg",
            "Please select an Item Type.");

        return;
    }

    const res = await postJson(
        "/ItemSubType/Create",
        {
            itemSubTypeName: subTypeName,
            itemTypeID,
            sort
        });

    if (res.success) {

        closeModal("createModal");

        showToast(
            res.message,
            "create");

        await loadSubTypes();

    } else {

        showError(
            "createErrorMsg",
            res.message);
    }
}

async function openEditModal(id) {

    const res = await getJson(
        `/ItemSubType/GetSubTypeForEdit?id=${id}`);

    if (!res.success) {

        showToast(
            res.message,
            "error");

        return;
    }

    document.getElementById(
        "edit_subTypeID").value =
        res.data.itemSubTypeID;

    document.getElementById(
        "edit_subTypeName").value =
        res.data.itemSubTypeName;

    document.getElementById(
        "edit_sort").value =
        res.data.sort;

    populateItemTypeDropdown(
        "edit_itemType",
        res.data.itemTypeID);

    clearValidation("edit");

    openModal("editModal");
}

async function saveEdit() {

    clearValidation("edit");

    const subTypeName =
        document.getElementById(
            "edit_subTypeName")
            .value
            .trim();

    const itemTypeID = parseInt(
        document.getElementById(
            "edit_itemType").value) || 0;

    const sort = parseInt(
        document.getElementById(
            "edit_sort").value) || 0;

    const subTypeID = parseInt(
        document.getElementById(
            "edit_subTypeID").value);

    if (!subTypeName) {

        markInvalid("edit_subTypeName");

        showError(
            "editErrorMsg",
            "Sub Type Name is required.");

        return;
    }

    if (itemTypeID === 0) {

        markInvalid("edit_itemType");

        showError(
            "editErrorMsg",
            "Please select an Item Type.");

        return;
    }

    const res = await postJson(
        "/ItemSubType/Update",
        {
            itemSubTypeID: subTypeID,
            itemSubTypeName: subTypeName,
            itemTypeID,
            sort
        });

    if (res.success) {

        closeModal("editModal");

        showToast(
            res.message,
            "edit");

        await loadSubTypes();

    } else {

        showError(
            "editErrorMsg",
            res.message);
    }
}

function openDeleteModal() {

    document.getElementById(
        "deleteCount").textContent =
        SubTypeState.selectedIds.length;

    openModal("deleteModal");
}

async function deleteSubTypes() {

    const res = await postJson(
        "/ItemSubType/DeleteSubTypes",
        {
            ids: SubTypeState.selectedIds
        });

    if (res.success) {

        closeModal("deleteModal");

        SubTypeState.selectedIds = [];

        showToast(
            res.message,
            "delete");

        await loadSubTypes();

    } else {

        showToast(
            res.message,
            "error");
    }
}

window.SubTypeModals = {
    openCreateModal,
    saveCreate,
    openEditModal,
    saveEdit,
    openDeleteModal,
    deleteSubTypes
};