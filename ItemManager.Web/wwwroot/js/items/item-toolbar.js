let _searchDebounce;

function initSearch() {
    const input =
        document.getElementById("searchInput");
    const clearBtn =
        document.getElementById("clearSearch");

    input.addEventListener("input", function () {
        clearBtn.style.display =
            this.value.length > 0 ? "inline-block" : "none";
        clearTimeout(_searchDebounce);
        _searchDebounce = setTimeout(() => {
            ItemState.search = this.value;
            ItemState.currentPage = 1;
            loadItems();
        }, 400);
    });

    clearBtn.addEventListener("click", () => {
        input.value = "";
        clearBtn.style.display = "none";
        ItemState.search = "";
        ItemState.currentPage = 1;
        loadItems();
    });
}

function initFilters() {
    document.getElementById("itemTypeFilter")
        .addEventListener("change", async function () {
            ItemState.itemTypeFilter =
                parseInt(this.value) || 0;
            ItemState.currentPage = 1;

            const subTypeSelect =
                document.getElementById(
                    "itemSubTypeFilter");
            subTypeSelect.innerHTML =
                '<option value="0">All SubTypes</option>';
            subTypeSelect.disabled = true;

            if (ItemState.itemTypeFilter > 0) {
                const res = await getJson(
                    `/Item/GetSubTypesByItemType?` +
                    `itemTypeId=${ItemState.itemTypeFilter}`);

                if (res && res.length > 0) {
                    res.forEach(st => {
                        const opt =
                            document.createElement("option");
                        opt.value = st.value;
                        opt.textContent = st.text;
                        subTypeSelect.appendChild(opt);
                    });
                    subTypeSelect.disabled = false;
                }
            }

            loadItems();
        });

    document.getElementById("itemSubTypeFilter")
        .addEventListener("change", function () {
            ItemState.itemSubTypeFilter =
                parseInt(this.value) || 0;
            ItemState.currentPage = 1;
            loadItems();
        });

    document.getElementById("conditionFilter")
        .addEventListener("change", function () {
            ItemState.conditionFilter = this.value;
            ItemState.currentPage = 1;
            loadItems();
        });
}