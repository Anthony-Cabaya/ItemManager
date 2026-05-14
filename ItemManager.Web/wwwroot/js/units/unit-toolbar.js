window.UnitToolbar = {

    update: function () {

        const hasCategory = !!UnitState.selectedCategoryId;
        const hasUnit = !!UnitState.selectedUnitId;
        const isSystem = UnitState.selectedUnitIsSystem === 'True';

        document.getElementById('btn-create-unit').disabled = !hasCategory;

        document.getElementById('btn-edit-unit').disabled = !hasUnit;

        document.getElementById('btn-delete-unit').disabled =
            !hasUnit || isSystem;
    }
};