let token = localStorage.getItem('token');

// Initialize UI based on token
document.addEventListener('DOMContentLoaded', () => {
    if (token) {
        showFileManager();
        listFiles();
    } else {
        showLogin();
    }
});

// Show login section
function showLogin() {
    document.getElementById('login-section').style.display = 'block';
    document.getElementById('register-section').style.display = 'none';
    document.getElementById('file-manager-section').style.display = 'none';
    document.getElementById('login-error').style.display = 'none';
}

// Show register section
function showRegister() {
    document.getElementById('login-section').style.display = 'none';
    document.getElementById('register-section').style.display = 'block';
    document.getElementById('file-manager-section').style.display = 'none';
    document.getElementById('register-error').style.display = 'none';
}

// Show file manager section
function showFileManager() {
    document.getElementById('login-section').style.display = 'none';
    document.getElementById('register-section').style.display = 'none';
    document.getElementById('file-manager-section').style.display = 'block';
    document.getElementById('file-error').style.display = 'none';
    document.getElementById('file-message').style.display = 'none';
}

// Register
document.getElementById('register-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const username = document.getElementById('register-username').value;
    const password = document.getElementById('register-password').value;
    const errorDiv = document.getElementById('register-error');

    try {
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const data = await response.json();
        if (data.token) {
            token = data.token;
            localStorage.setItem('token', token);
            showLogin();
            document.getElementById('login-username').value = username;
        } else {
            errorDiv.textContent = data.message || 'Registration failed';
            errorDiv.style.display = 'block';
        }
    } catch (err) {
        errorDiv.textContent = 'Registration failed';
        errorDiv.style.display = 'block';
    }
});

// Login
document.getElementById('login-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const username = document.getElementById('login-username').value;
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const data = await response.json();
        if (data.token) {
            token = data.token;
            localStorage.setItem('token', token);
            showFileManager();
            listFiles();
        } else {
            errorDiv.textContent = data.message || 'Login failed';
            errorDiv.style.display = 'block';
        }
    } catch (err) {
        errorDiv.textContent = 'Login failed';
        errorDiv.style.display = 'block';
    }
});

// Upload file
document.getElementById('upload-form').addEventListener('submit', async (e) => {
    e.preventDefault();
    const fileInput = document.getElementById('file-input');
    const fileName = document.getElementById('file-name').value;
    const errorDiv = document.getElementById('file-error');
    const messageDiv = document.getElementById('file-message');

    if (!fileInput.files[0]) {
        errorDiv.textContent = 'Please select a file';
        errorDiv.style.display = 'block';
        return;
    }

    const formData = new FormData();
    formData.append('file', fileInput.files[0]);
    if (fileName) formData.append('name', fileName);

    try {
        const response = await fetch('/api/files/upload', {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            body: formData
        });
        const data = await response.json();
        if (response.ok) {
            messageDiv.textContent = data.message;
            messageDiv.style.display = 'block';
            errorDiv.style.display = 'none';
            fileInput.value = '';
            document.getElementById('file-name').value = '';
            listFiles();
        } else {
            errorDiv.textContent = data.message || 'Upload failed';
            errorDiv.style.display = 'block';
            if (response.status === 401) {
                localStorage.removeItem('token');
                token = null;
                showLogin();
            }
        }
    } catch (err) {
        errorDiv.textContent = 'Upload failed';
        errorDiv.style.display = 'block';
    }
});

// Download file (latest or specific revision)
function downloadFile(fileName, revision = null) {
    let url = `/api/files/${encodeURIComponent(fileName)}`;
    let downloadName = fileName;

    if (revision !== null && revision !== '') {
        url += `?revision=${revision}`;
        // Append version to filename, e.g., review-v0.pdf
        const extensionIndex = fileName.lastIndexOf('.');
        if (extensionIndex !== -1) {
            downloadName = `${fileName.substring(0, extensionIndex)}-v${revision}${fileName.substring(extensionIndex)}`;
        } else {
            downloadName = `${fileName}-v${revision}`;
        }
    }

    fetch(url, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('token');
                    token = null;
                    showLogin();
                    throw new Error('Unauthorized access');
                }
                return response.json().then(data => {
                    throw new Error(data.message || 'Download failed');
                });
            }
            return response.blob();
        })
        .then(blob => {
            const downloadLink = document.createElement('a');
            downloadLink.href = window.URL.createObjectURL(blob);
            downloadLink.download = downloadName;
            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);
        })
        .catch(error => {
            const errorDiv = document.getElementById('file-error');
            errorDiv.textContent = error.message;
            errorDiv.style.display = 'block';
        });
}

// List files
async function listFiles() {
    const fileList = document.getElementById('file-list');
    const errorDiv = document.getElementById('file-error');
    try {
        const response = await fetch('/api/files', {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        if (response.ok) {
            fileList.innerHTML = '';
            data.files.forEach(file => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${file.name}</td>
                    <td>${file.version}</td>
                    <td>${new Date(file.uploadDate).toLocaleString()}</td>
                    <td>
                        <button onclick="downloadFile('${file.name}')" class="btn btn-sm btn-primary">Download Latest</button>
                        <input type="number" min="0" max="${file.version}" placeholder="Revision" style="width: 80px; display: inline-block;" onchange="downloadFile('${file.name}', this.value)" />
                    </td>
                `;
                fileList.appendChild(row);
            });
        } else {
            errorDiv.textContent = data.message || 'Failed to fetch files';
            errorDiv.style.display = 'block';
            if (response.status === 401) {
                localStorage.removeItem('token');
                token = null;
                showLogin();
            }
        }
    } catch (err) {
        errorDiv.textContent = 'Failed to fetch files';
        errorDiv.style.display = 'block';
    }
}

// Logout
function logout() {
    localStorage.removeItem('token');
    token = null;
    showLogin();
}