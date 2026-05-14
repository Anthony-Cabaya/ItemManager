function getField(id) {

    return document.getElementById(id);
}

function getValue(id) {

    return getField(id)?.value ?? "";
}

function setValue(id, value) {

    const field = getField(id);

    if (field) {
        field.value = value;
    }
}

function showError(divId, message) {

    const div = getField(divId);

    if (!div)
        return;

    div.textContent = message;
    div.classList.remove("d-none");

    const modal = div.closest(".modal");

    if (modal)
        modal.scrollTop = 0;

    div.focus();
}

function markInvalid(fieldId) {

    getField(fieldId) ?.classList.add("is-invalid");
}

function clearValidation(prefix) {

    [
        `${prefix}_categoryName`,
        `${prefix}_sort`
    ].forEach(id => {

        getField(id) ?.classList.remove("is-invalid");
    });

    getField(`${prefix}ErrorMsg`) ?.classList.add("d-none");
}

function toggleSystemEditMode(isSystem) {

    const warning = getField("systemWarning");
    const nameInput = getField("edit_categoryName");

    const label = nameInput
        ?.closest(".mb-2")
        ?.querySelector("label");

    if (!warning || !nameInput)
        return;

    if (isSystem) {

        warning.classList.remove("d-none");
        nameInput.disabled = true;
        label?.classList.add("text-muted");

    } else {

        warning.classList.add("d-none");
        nameInput.disabled = false;
        label?.classList.remove("text-muted");
    }
}

async function openCreateModal() {

    setValue("create_categoryName", "");
    setValue("create_sort", "0");
    clearValidation("create");
    openModal("createModal");
}

async function saveCreate() {

    clearValidation("create");

    const categoryName = getValue("create_categoryName").trim();
    const sort = parseInt(getValue("create_sort")) || 0;

    if (!categoryName) {

        markInvalid("create_categoryName");
        showError("createErrorMsg", "Category Name is required.");

        return;
    }

    const res = await postJson(
        "/UnitCategory/Create",
        {
            categoryName,
            sort
        });

    if (res.success) {

        closeModal("createModal");
        showToast(res.message, "create");
        await loadCategories();

    } else {

        showError("createErrorMsg", res.message);
    }
}

async function openEditModal(id) {

    const res = await getJson(`/UnitCategory/GetCategoryForEdit?id=${id}`);

    if (!res.success) {

        showToast(res.message, "error");

        return;
    }

    setValue("edit_categoryID", res.data.unitCategoryID);
    setValue("edit_isSystem", res.data.isSystem);
    setValue("edit_categoryName", res.data.categoryName);
    setValue("edit_sort", res.data.sort);
    toggleSystemEditMode(res.data.isSystem === true);
    clearValidation("edit");
    openModal("editModal");
}

async function saveEdit() {

    clearValidation("edit");

    const categoryID = parseInt(getValue("edit_categoryID"));
    const isSystem = getValue("edit_isSystem") === "true";
    const sort = parseInt(getValue("edit_sort")) || 0;

    let categoryName = "";

    if (!isSystem) {

        categoryName = getValue("edit_categoryName").trim();

        if (!categoryName) {

            markInvalid("edit_categoryName");
            showError("editErrorMsg", "Category Name is required.");

            return;
        }

    } else {

        categoryName = getValue("edit_categoryName");
    }

    const res = await postJson(
        "/UnitCategory/Update",
        {
            unitCategoryID:
                categoryID,
            categoryName,
            sort
        });

    if (res.success) {

        closeModal("editModal");
        showToast(res.message, "edit");

        await loadCategories();

    } else {

        showError("editErrorMsg", res.message);
    }
}

function openDeleteModal() {

    getField("deleteCount").textContent = CategoryState.selectedIds.length;
    openModal("deleteModal");
}

async function deleteCategories() {

    const res = await postJson(
        "/UnitCategory/DeleteCategories",
        {
            ids:CategoryState.selectedIds
        });

    if (res.success) {

        closeModal("deleteModal");

        CategoryState.selectedIds = [];

        showToast(res.message, "delete");

        await loadCategories();

    } else {

        showToast(res.message, "error");
    }
}

window.CategoryModals = {
    openCreateModal,
    saveCreate,
    openEditModal,
    saveEdit,
    openDeleteModal,
    deleteCategories
};