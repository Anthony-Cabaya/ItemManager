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

        const style = document.createElement('style');
        style.innerHTML = `
            .active-category {
                background-color: #0d6efd;
                color: #fff;
                border-radius: 4px;
            }
        `;
        document.head.appendChild(style);
    }
};