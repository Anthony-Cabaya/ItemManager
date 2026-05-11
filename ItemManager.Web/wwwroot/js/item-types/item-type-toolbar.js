function initItemTypeSearch() {

    const input =
        document.getElementById("searchInput");

    const clearBtn =
        document.getElementById("clearSearch");

    input.addEventListener("input", function () {

        clearBtn.style.display =
            this.value ? "inline" : "none";
    });

    clearBtn.addEventListener("click", function () {

        input.value = "";
        clearBtn.style.display = "none";

        ItemTypeState.search = "";
        ItemTypeState.currentPage = 1;

        loadItemTypes();
    });

    input.addEventListener("keydown", function (e) {

        if (e.key === "Enter") {

            ItemTypeState.search = this.value;
            ItemTypeState.currentPage = 1;

            loadItemTypes();
        }
    });

    document.getElementById("btnSearch")
        .addEventListener("click", function () {

            ItemTypeState.search = input.value;
            ItemTypeState.currentPage = 1;

            loadItemTypes();
        });
}