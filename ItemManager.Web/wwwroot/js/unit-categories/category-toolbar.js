function initCategorySearch() {

    const input =
        document.getElementById("searchInput");

    const clearBtn =
        document.getElementById("clearSearch");

    const btnSearch =
        document.getElementById("btnSearch");

    input.addEventListener("input", () => {

        clearBtn.style.display =
            input.value.length > 0
                ? "block"
                : "none";
    });

    clearBtn.addEventListener("click", async () => {

        input.value = "";
        clearBtn.style.display = "none";

        CategoryState.search = "";
        CategoryState.currentPage = 1;

        await loadCategories();
    });

    input.addEventListener("keydown",
        async e => {

            if (e.key !== "Enter")
                return;

            CategoryState.search =
                input.value.trim();

            CategoryState.currentPage = 1;

            await loadCategories();
        });

    btnSearch.addEventListener("click",
        async () => {

            CategoryState.search =
                input.value.trim();

            CategoryState.currentPage = 1;

            await loadCategories();
        });
}