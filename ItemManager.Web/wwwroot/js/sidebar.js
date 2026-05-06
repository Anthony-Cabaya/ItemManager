document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("sidebar");
    const overlay = document.getElementById("sidebarOverlay");
    const hamburger = document.getElementById("hamburger");
    const closeBtn = document.getElementById("sidebarCloseBtn");

    function openSidebar() {
        sidebar.classList.add("mobile-open");
        overlay.classList.add("active");
        document.body.style.overflow = "hidden";
    }

    function closeSidebar() {
        sidebar.classList.remove("mobile-open");
        overlay.classList.remove("active");
        document.body.style.overflow = "";
    }

    if (hamburger)
        hamburger.addEventListener("click", openSidebar);

    if (closeBtn)
        closeBtn.addEventListener("click", closeSidebar);

    if (overlay)
        overlay.addEventListener("click", closeSidebar);

    document.querySelectorAll("#sidebar .nav-item")
        .forEach(item => {
            item.addEventListener("click", function () {
                if (window.innerWidth <= 768) {
                    closeSidebar();
                }
            });
        });

});