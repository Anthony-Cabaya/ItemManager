document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("sidebar");
    const mainContent = document.getElementById("mainContent");
    const overlay = document.getElementById("sidebarOverlay");
    const hamburger = document.getElementById("hamburger");
    const toggleBtn = document.getElementById("sidebarToggle");

    const BREAKPOINT = 768;

    let isMobile = window.innerWidth <= BREAKPOINT;

    function updateToggleIcon() {
        if (!toggleBtn) return;

        toggleBtn.textContent =
            sidebar.classList.contains("collapsed") ? "▶" : "◀";
    }

    // INIT STATE (DESKTOP COLLAPSE)
    function initSidebarState() {
        const saved = localStorage.getItem("sidebarCollapsed");

        if (!isMobile && saved === "true") {
            sidebar.classList.add("collapsed");
        }

        updateToggleIcon();
        closeMobileSidebar();
    }

    // TOGGLE DESKTOP COLLAPSE
    function toggleCollapse() {
        sidebar.classList.toggle("collapsed");

        const isCollapsed = sidebar.classList.contains("collapsed");
        localStorage.setItem("sidebarCollapsed", isCollapsed);
    }

    // OPEN MOBILE SIDEBAR
    function openMobileSidebar() {
        sidebar.classList.add("mobile-open");
        overlay.classList.add("active");
    }

    // CLOSE MOBILE SIDEBAR
    function closeMobileSidebar() {
        sidebar.classList.remove("mobile-open");
        overlay.classList.remove("active");
    }

    // HANDLE RESPONSIVE MODE
    function handleResize() {
        const nowMobile = window.innerWidth <= BREAKPOINT;

        if (nowMobile !== isMobile) {
            isMobile = nowMobile;

            closeMobileSidebar();

            if (isMobile) {
                sidebar.classList.remove("collapsed");
            } else {
                initSidebarState();
            }
        }
    }

    // EVENTS

    // desktop collapse toggle
    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            toggleCollapse();
            updateToggleIcon();
        });
    }

    // mobile side
    if (hamburger) {
        hamburger.addEventListener("click", function () {
            openMobileSidebar();
        });
    }

    if (overlay) {
        overlay.addEventListener("click", function () {
            closeMobileSidebar();
        });
    }

    document.querySelectorAll("#sidebar .nav-item").forEach(item => {
        item.addEventListener("click", function () {
            if (isMobile) {
                closeMobileSidebar();
            }
        });
    });

    window.addEventListener("resize", handleResize);

    // INIT RUN
    initSidebarState();

});