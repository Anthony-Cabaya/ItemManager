document.addEventListener("DOMContentLoaded", () => {

    const addError = document.getElementById("add-variant-error");
    const editError = document.getElementById("edit-variant-error");
    const deleteError = document.getElementById("delete-variant-error");
    const addBtn = document.getElementById("btn-confirm-add-variant");
    const editBtn = document.getElementById("btn-update-variant");
    const deleteBtn = document.getElementById("btn-confirm-delete-variant");

    function hideError(el) {

        if (!el)
            return;

        el.style.visibility = "hidden";
        el.style.padding = "0";
        el.style.margin = "0";
        el.style.border = "none";
        el.style.minHeight = "0";
        el.textContent = "";
    }

    function showError(el, message) {

        if (!el)
            return;

        el.textContent = message;

        el.style.visibility = "visible";
        el.style.padding = "";
        el.style.margin = "";
        el.style.border = "";
        el.style.minHeight = "";
    }

    function safeModalHide(modalId, callback) {

        document.activeElement?.blur();

        const modalEl =
            document.getElementById(modalId);

        if (!modalEl) {

            if (callback)
                callback();

            return;
        }

        function onHidden() {

            modalEl.removeEventListener(
                "hidden.bs.modal",
                onHidden);

            if (callback)
                callback();
        }

        modalEl.addEventListener("hidden.bs.modal", onHidden);

        const instance =
            bootstrap.Modal.getInstance(modalEl) ||
            new bootstrap.Modal(modalEl);

        instance.hide();
    }

    if (addBtn) {

        addBtn.addEventListener(
            "click",
            async () => {

                hideError(addError);

                const payload = {

                    itemID: parseInt(
                        document.getElementById(
                            "add-variant-item-id")
                            .value),

                    variantCode:
                        document.getElementById(
                            "add-variant-code")
                            .value
                            .trim(),

                    variantName:
                        document.getElementById(
                            "add-variant-name")
                            .value
                            .trim(),

                    attributesText:
                        document.getElementById(
                            "add-variant-attributes")
                            .value
                            .trim(),

                    isActive:
                        document.getElementById(
                            "add-variant-is-active")
                            .checked
                };

                if (!payload.variantCode || !payload.variantName)
                {
                    showError(addError, "Variant Code and Name are required.");
                    return;
                }

                try {

                    const res = await fetch(
                        "/ItemVariant/AddSingle",
                        {
                            method: "POST",

                            headers: {
                                "Content-Type":
                                    "application/json",

                                "RequestVerificationToken":
                                    getAntiForgeryToken()
                            },

                            body: JSON.stringify(payload)
                        });

                    const data = await res.json();

                    if (!data.success) {

                        showError(addError, data.message || "Failed.");
                        return;
                    }

                    safeModalHide(
                        "addVariantModal",
                        () => {

                            toast(data.message, "add");

                            window.reloadVariantTable();

                            window.VariantState
                                ?.clear?.();
                        });

                } catch {

                    showError(addError, "Server error.");
                }
            });
    }

    if (editBtn) {

        editBtn.addEventListener(
            "click",
            async () => {

                hideError(editError);

                const editCode =
                    document.getElementById(
                        "variant-edit-code")
                        .value
                        .trim();

                const originalCode =
                    document.getElementById(
                        "variant-edit-original-code")
                        .value;

                const finalCode = editCode || originalCode;

                document.getElementById(
                    "variant-edit-code")
                    .value = finalCode;

                const payload = {

                    itemVariantID: parseInt(
                        document.getElementById(
                            "variant-edit-id")
                            .value),

                    itemID: parseInt(
                        document.getElementById(
                            "variant-edit-item-id")
                            .value),

                    variantCode: finalCode,

                    variantName:
                        document.getElementById(
                            "variant-edit-name")
                            .value
                            .trim(),

                    attributesText:
                        document.getElementById(
                            "variant-edit-attributes")
                            .value
                            .trim(),

                    isActive:
                        document.getElementById(
                            "variant-edit-is-active")
                            .checked,

                    sort: 0
                };

                try {

                    const res = await fetch(
                        "/ItemVariant/Edit",
                        {
                            method: "POST",

                            headers: {
                                "Content-Type":
                                    "application/json",

                                "RequestVerificationToken":
                                    getAntiForgeryToken()
                            },

                            body: JSON.stringify(payload)
                        });

                    const data =
                        await res.json();

                    if (!data.success) {

                        showError(editError, data.message || "Update failed.");
                        return;
                    }

                    safeModalHide("editVariantModal", async () => {
                        toast(data.message, "edit");

                        await window.reloadVariantTable();

                        window.VariantState?.clear?.();
                        window.VariantState?.updateUI?.();
                    });

                } catch {

                    showError(editError, "Server error.");
                }
            });
    }

    if (deleteBtn) {

        deleteBtn.addEventListener(
            "click",
            async () => {

                hideError(deleteError);

                const rawId =
                    document.getElementById(
                        "delete-variant-id")
                        .value;

                const ids =
                    rawId
                        .split(",")
                        .map(x => parseInt(x.trim()))
                        .filter(x => !isNaN(x));

                let lastMessage = "Deleted successfully.";

                try {

                    for (const variantId of ids) {

                        const res = await fetch(
                            "/ItemVariant/Delete",
                            {
                                method: "POST",

                                headers: {
                                    "Content-Type":
                                        "application/json",

                                    "RequestVerificationToken":
                                        getAntiForgeryToken()
                                },

                                body: JSON.stringify({
                                    variantId
                                })
                            });

                        const data = await res.json();

                        if (!data.success) {

                            showError(
                                deleteError,
                                data.message || "Delete failed."
                            );

                            return;
                        }

                        lastMessage =
                            data.message || lastMessage;
                    }

                    safeModalHide(
                        "deleteVariantModal",
                        () => {

                            toast(lastMessage, "delete");

                            window.reloadVariantTable();

                            window.VariantState?.clear?.();
                        });

                } catch {

                    showError(deleteError, "Server error.");
                }
            });
    }

});