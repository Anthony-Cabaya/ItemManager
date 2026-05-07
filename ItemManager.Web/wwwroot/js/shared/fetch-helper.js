function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? "";
}

async function postJson(url, data) {
    const res = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": getAntiForgeryToken()
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        throw new Error(`POST request failed: ${res.status}`);
    }

    return await res.json();
}

async function getJson(url) {
    const res = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "application/json"
        }
    });

    if (!res.ok) {
        throw new Error(`GET request failed: ${res.status}`);
    }

    return await res.json();
}

// Export globally
window.getAntiForgeryToken = getAntiForgeryToken;
window.postJson = postJson;
window.getJson = getJson;