window.UnitCategory = {

    initCategoryList: function () {
        const items = document.querySelectorAll('.category-item');

        items.forEach(item => {
            item.addEventListener('click', () => {

                // state
                UnitState.selectedCategoryId = item.dataset.categoryId;
                UnitState.selectedCategoryIsSystem = item.dataset.isSystem === 'True';

                UnitState.selectedUnitId = null;
                UnitState.selectedUnitName = '';
                UnitState.selectedUnitIsSystem = '';

                // UI highlight
                items.forEach(x => x.classList.remove('active-category'));
                item.classList.add('active-category');

                // load table
                UnitTable.loadByCategory(UnitState.selectedCategoryId);

                UnitToolbar.update();
            });
        });

        if (!document.getElementById('unit-category-styles')) {

            const style = document.createElement('style');

            style.id = 'unit-category-styles';

            style.innerHTML = `
                .category-item {
                    border-left: 3px solid transparent;
                    border-radius: 4px;
                    transition: background-color 0.15s;
                }

                .category-item:hover {
                    background-color: #f0f4ff;
                }

                .active-category {
                    background-color: #e7f1ff !important;
                    border-left: 3px solid #0d6efd !important;
                    color: #0d6efd !important;
                    font-weight: 600;
                    border-radius: 4px;
                }
            `;

            document.head.appendChild(style);
        }
    }
};