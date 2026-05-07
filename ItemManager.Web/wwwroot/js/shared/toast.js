function showToast(message, type = "default") {
    // Toast container
    let container = document.getElementById("toastContainer");
    if (!container) {
        container = document.createElement("div");
        container.id = "toastContainer";
        Object.assign(container.style, {
            position: "fixed",
            top: "20px",
            right: "20px",
            zIndex: "9999",
            display: "flex",
            flexDirection: "column",
            gap: "8px"
        });
        document.body.appendChild(container);
    }

    // Type styles
    const typeStyles = {
        create: { bg: "#dbeafe", border: "#1d4ed8", color: "#1e3a8a" },
        edit: { bg: "#fef9c3", border: "#ca8a04", color: "#713f12" },
        delete: { bg: "#fee2e2", border: "#b91c1c", color: "#7f1d1d" },
        error: { bg: "#fee2e2", border: "#dc3545", color: "#7f1d1d" },
        info: { bg: "#f0fdf4", border: "#15803d", color: "#14532d" },
        default: { bg: "#f1f5f9", border: "#64748b", color: "#1e293b" }
    };

    const style = typeStyles[type] || typeStyles.default;

    // Create toast element
    const toast = document.createElement("div");
    Object.assign(toast.style, {
        padding: "12px 16px",
        borderRadius: "8px",
        borderLeft: `4px solid ${style.border}`,
        background: style.bg,
        color: style.color,
        fontSize: "13px",
        minWidth: "240px",
        maxWidth: "320px",
        boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
        opacity: "1",
        transition: "opacity 0.4s"
    });

    toast.textContent = message;

    container.appendChild(toast);

    // Auto-dismiss
    setTimeout(() => {
        toast.style.opacity = "0";
        setTimeout(() => {
            toast.remove();
        }, 400);
    }, 3000);
}

// Export globally
window.showToast = showToast;