// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Global Helper Function to Show Toasts
function showToast(message, isSuccess) {
    const toastElement = document.getElementById('liveToast');
    const toastMessage = document.getElementById('toastMessage');

    // Set message and dynamic colors based on success/error
    toastMessage.innerText = message;
    toastElement.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'bg-info');
    toastElement.classList.add(isSuccess ? 'bg-success' : 'bg-danger');

    // Initialize and show via Bootstrap's JS API
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}

// Delete Confirmation with AJAX
function confirmDelete(url, itemType = 'item') {
    if (confirm(`Are you sure you want to delete this ${itemType}? This action cannot be undone.`)) {
        // Make AJAX DELETE request
        fetch(url, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => {
            if (response.ok) {
                showToast(`${itemType} deleted successfully!`, true);
                setTimeout(() => window.location.reload(), 1000);
            } else {
                showToast(`Failed to delete ${itemType}. Please try again.`, false);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast(`An error occurred while deleting ${itemType}.`, false);
        });
        return false;
    }
    return false;
}

// AJAX Form Submit with Notification
function submitFormAjax(formSelector, actionUrl, successMessage) {
    const form = document.querySelector(formSelector);
    if (!form) return;

    form.addEventListener('submit', function(e) {
        e.preventDefault();

        const formData = new FormData(form);

        fetch(actionUrl, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => {
            if (response.ok) {
                showToast(successMessage || 'Operation completed successfully!', true);
                setTimeout(() => window.location.reload(), 1500);
            } else {
                showToast('An error occurred. Please try again.', false);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('An error occurred while submitting the form.', false);
        });
    });
}

// Sidebar Toggler for Mobile
document.addEventListener("DOMContentLoaded", function () {
    const sidebarToggle = document.getElementById("sidebarToggle");
    const mainWrapper = document.querySelector(".saas-main-wrapper");
    if (sidebarToggle && mainWrapper) {
        sidebarToggle.addEventListener("click", function () {
            mainWrapper.classList.toggle("sidebar-open");
        });
    }
});