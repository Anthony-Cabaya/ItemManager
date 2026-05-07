function applyColumnVisibility(visibleCols) {
    document.querySelectorAll("[data-col]").forEach(el => {
        const col = el.dataset.col;
        if (visibleCols.includes(col)) {
            el.classList.remove("d-none");
        } else {
            el.classList.add("d-none");
        }
    });
}
function initColumns(storageKey, defaultVisible) {
    let saved = localStorage.getItem(storageKey);
    let visibleCols = saved ? JSON.parse(saved) : defaultVisible;

    // Apply visibility
    applyColumnVisibility(visibleCols);

    // Sync checkboxes
    document.querySelectorAll(".col-toggle").forEach(cb => {
        cb.checked = visibleCols.includes(cb.dataset.col);
        cb.addEventListener("change", () => saveColumnPrefs(storageKey));
    });
}

function saveColumnPrefs(storageKey) {
    const checked = [];
    document.querySelectorAll(".col-toggle").forEach(cb => {
        if (cb.checked) checked.push(cb.dataset.col);
    });
    localStorage.setItem(storageKey, JSON.stringify(checked));
    applyColumnVisibility(checked);
}
function toggleColumnPanel(panelId) {
    const panel = document.getElementById(panelId);
    if (!panel) return;
    panel.style.display = panel.style.display === "none" ? "block" : "none";
}

// Close column panel when clicking outside
document.addEventListener("click", e => {
    const panel = document.getElementById("columnPanel");
    const btn = document.getElementById("btnColumns");
    if (panel && btn && !panel.contains(e.target) && !btn.contains(e.target)) {
        panel.style.display = "none";
    }
});

// Export globally
window.applyColumnVisibility = applyColumnVisibility;
window.initColumns = initColumns;
window.saveColumnPrefs = saveColumnPrefs;
window.toggleColumnPanel = toggleColumnPanel;