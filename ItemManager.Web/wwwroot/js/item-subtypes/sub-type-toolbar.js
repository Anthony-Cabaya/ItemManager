function initSubTypeSearch() {

    const input =
        document.getElementById("searchInput");

    const clearBtn =
        document.getElementById("clearSearch");

    if (!input || !clearBtn) {
        return;
    }

    input.addEventListener("input", function () {

        clearBtn.style.display =
            input.value.trim()
                ? "block"
                : "none";
    });

    clearBtn.addEventListener("click", async function () {

        input.value = "";
        clearBtn.style.display = "none";

        SubTypeState.search = "";
        SubTypeState.currentPage = 1;

        await loadSubTypes();
    });

    input.addEventListener("keydown",
        async function (e) {

            if (e.key !== "Enter") {
                return;
            }

            SubTypeState.search =
                input.value.trim();

            SubTypeState.currentPage = 1;

            await loadSubTypes();
        });

    const btnSearch =
        document.getElementById("btnSearch");

    if (btnSearch) {

        btnSearch.addEventListener("click",
            async function () {

                SubTypeState.search =
                    input.value.trim();

                SubTypeState.currentPage = 1;

                await loadSubTypes();
            });
    }
}

function initSubTypeFilters() {

    const pageData =
        document.getElementById("pageData");

    const select =
        document.getElementById("itemTypeFilter");

    if (!pageData || !select) {
        return;
    }

    const itemTypes = JSON.parse(
        pageData.dataset.itemTypes || "[]");

    itemTypes.forEach(t => {

        const opt =
            document.createElement("option");

        opt.value = t.value;
        opt.textContent = t.text;

        select.appendChild(opt);
    });

    select.addEventListener("change",
        async function () {

            SubTypeState.itemTypeFilter =
                parseInt(this.value) || 0;

            SubTypeState.currentPage = 1;

            await loadSubTypes();
        });
}