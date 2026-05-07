const _modalInstances = {};

function openModal(modalId) {
    if (!_modalInstances[modalId]) {
        const el = document.getElementById(modalId);
        if (!el) {
            console.warn(`Modal element with ID "${modalId}" not found.`);
            return;
        }
        _modalInstances[modalId] = new bootstrap.Modal(el);
    }
    _modalInstances[modalId].show();
}

function closeModal(modalId) {
    if (_modalInstances[modalId]) {
        _modalInstances[modalId].hide();
    }
}

// Export globally
window.openModal = openModal;
window.closeModal = closeModal;