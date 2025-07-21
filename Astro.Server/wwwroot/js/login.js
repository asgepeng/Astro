async function tryLoginAsync() {
    const username = document.getElementById('username')?.value ?? '';
    const password = document.getElementById('password')?.value ?? '';

    try {
        const response = await fetch('/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        const statusCode = response.status;

        // Coba ambil hasil hanya jika ada konten JSON
        let result = null;
        const contentType = response.headers.get('content-type') || '';
        if (contentType.includes('application/json')) {
            result = await response.json();
        }

        switch (statusCode) {
            case 401:
                alert("Unauthorized Request");
                break;
            case 403:
                alert("Your password currently expired");
                break;
            case 500:
                alert(result?.detail ?? 'Internal Server Error');
                break;
        }

        if (response.ok && result?.accessToken) {
            localStorage.setItem('accessToken', result.accessToken);
            await getDataProductAsync('/data/products');
        } else if (!response.ok) {
            alert('Login gagal (status: ' + statusCode + '): ' + (result?.message ?? ''));
        }

    } catch (err) {
        alert('Request error: ' + err.message);
    }
}
async function getDataProductAsync(url) {
    const token = localStorage.getItem('accessToken');
    if (!token) {
        alert("No access token found. Please login first.");
        return;
    }

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            alert('Failed to fetch product data. Status: ' + response.status);
            return;
        }

        const htmlContent = await response.text();
        const container = document.getElementById('main-container');
        if (container) {
            container.innerHTML = htmlContent;
            container.querySelectorAll('.page-link').forEach(item => {
                item.addEventListener('click', function () {
                    await getDataProductAsync('/data/products?pg=' + item.textContent);
                })
            });
        } else {
            console.log("HTML Content:", htmlContent);
        }

    } catch (err) {
        alert('Request error: ' + err.message);
    }
}



document.addEventListener('DOMContentLoaded', function () {
    (async () => {
        const loginButton = document.getElementById('loginButton');
        if (loginButton) {
            loginButton.addEventListener('click', async function () {
                await tryLoginAsync();
            });
        }

        const token = localStorage.getItem('accessToken');
        if (token) {
            await getDataProductAsync('/data/products');
        }
    })();
});
