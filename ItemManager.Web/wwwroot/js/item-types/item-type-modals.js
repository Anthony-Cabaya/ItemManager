function showError(divId, message) {

    const div = document.getElementById(divId);

    if (!div) return;

    div.style.display = "block";
    div.textContent = message;

    const modal = div.closest(".modal");

    if (modal) {
        modal.scrollTop = 0;
        div.focus();
    }
}

function markInvalid(fieldId) {

    const el = document.getElementById(fieldId);

    if (el) {
        el.classList.add("is-invalid");
    }
}

function clearValidation(prefix) {

    const fields = [
        `${prefix}_itemTypeName`,
        `${prefix}_sort`
    ];

    fields.forEach(id => {

        const el = document.getElementById(id);

        if (el) {
            el.classList.remove("is-invalid");
        }
    });

    const err = document.getElementById(`${prefix}ErrorMsg`);

    if (err) {
        err.style.display = "none";
        err.textContent = "";
    }
}

async function openCreateModal() {

    document.getElementById("create_itemTypeName").value = "";
    document.getElementById("create_sort").value = "0";

    clearValidation("create");

    openModal("createModal");
}

async function saveCreate() {

    clearValidation("create");

    const itemTypeName =
        document.getElementById("create_itemTypeName").value;

    if (!itemTypeName) {

        markInvalid("create_itemTypeName");

        showError(
            "createErrorMsg",
            "Item Type Name is required.");

        return;
    }

    const sort =
        document.getElementById("create_sort").value;

    const res = await postJson("/ItemType/Create", {
        itemTypeName,
        sort: parseInt(sort) || 0
    });

    if (res.success) {

        closeModal("createModal");
        showToast(res.message, "create");
        loadItemTypes();
    }
    else {
        showError("createErrorMsg", res.message);
    }
}

async function openEditModal(id) {

    const res = await getJson(
        `/ItemType/GetItemTypeForEdit?id=${id}`);

    if (!res.success) {
        showToast(res.message, "error");
        return;
    }

    document.getElementById("edit_itemTypeID").value =
        res.data.itemTypeID;

    document.getElementById("edit_itemTypeName").value =
        res.data.itemTypeName;

    document.getElementById("edit_sort").value =
        res.data.sort;

    clearValidation("edit");

    openModal("editModal");
}

async function saveEdit() {

    clearValidation("edit");

    const itemTypeName =
        document.getElementById("edit_itemTypeName").value;

    if (!itemTypeName) {

        markInvalid("edit_itemTypeName");

        showError(
            "editErrorMsg",
            "Item Type Name is required.");

        return;
    }

    const itemTypeID =
        document.getElementById("edit_itemTypeID").value;

    const sort =
        document.getElementById("edit_sort").value;

    const res = await postJson("/ItemType/Update", {
        itemTypeID: parseInt(itemTypeID),
        itemTypeName,
        sort: parseInt(sort) || 0
    });

    if (res.success) {

        closeModal("editModal");
        showToast(res.message, "edit");
        loadItemTypes();
    }
    else {
        showError("editErrorMsg", res.message);
    }
}

function openDeleteModal() {

    document.getElementById("deleteCount").textContent =
        ItemTypeState.selectedIds.length;

    openModal("deleteModal");
}

async function deleteItemTypes() {

    const res = await postJson(
        "/ItemType/DeleteItemTypes",
        { ids: ItemTypeState.selectedIds });

    if (res.success) {

        closeModal("deleteModal");

        ItemTypeState.selectedIds = [];

        showToast(res.message, "delete");

        loadItemTypes();
    }
    else {
        showToast(res.message, "error");
    }
}

window.ItemTypeModals = {
    openCreateModal,
    saveCreate,
    openEditModal,
    saveEdit,
    openDeleteModal,
    deleteItemTypes
};