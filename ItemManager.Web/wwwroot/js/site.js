window.toast = function (message, type = "success") {

    const toastEl = document.getElementById("global-toast");
    const toastBody = document.getElementById("global-toast-body");

    if (!toastEl || !toastBody) {
        console.log(message);
        return;
    }

    toastBody.textContent = message;

    const map = {
        success: "bg-success text-white",
        error: "bg-danger text-white",
        warning: "bg-warning text-dark",
        info: "bg-info text-dark",

        add: "bg-primary text-white",
        edit: "bg-warning text-dark",
        delete: "bg-danger text-white"
    };

    toastEl.className =
        "toast align-items-center border-0 " + (map[type] || map.success);

    const toast = bootstrap.Toast.getOrCreateInstance(toastEl, {
        delay: 2500
    });

    toast.show();
};