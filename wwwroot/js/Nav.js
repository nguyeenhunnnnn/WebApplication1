// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Get the menu toggle button and nav menu
    const menuToggle = document.querySelector('.menu-toggle');
    const navMenu = document.querySelector('.nav-menu');
    const menuOverlay = document.querySelector('.menu-overlay');

    // Toggle menu on button click
    if (menuToggle) {
        menuToggle.addEventListener('click', function () {
            menuToggle.classList.toggle('active');
            navMenu.classList.toggle('active');

            // Also toggle overlay
            if (menuOverlay) {
                menuOverlay.classList.toggle('active');
            }

            // Prevent scrolling when menu is open
            document.body.classList.toggle('no-scroll');
        });
    }

    // Close menu when clicking on overlay
    if (menuOverlay) {
        menuOverlay.addEventListener('click', function () {
            menuToggle.classList.remove('active');
            navMenu.classList.remove('active');
            menuOverlay.classList.remove('active');
            document.body.classList.remove('no-scroll');
        });
    }

    // Close menu when clicking on a nav item (for mobile)
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
        item.addEventListener('click', function () {
            if (window.innerWidth <= 768) {
                menuToggle.classList.remove('active');
                navMenu.classList.remove('active');
                if (menuOverlay) {
                    menuOverlay.classList.remove('active');
                }
                document.body.classList.remove('no-scroll');
            }
        });
    });

    // Add resize listener to reset menu state on window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            menuToggle.classList.remove('active');
            navMenu.classList.remove('active');
            if (menuOverlay) {
                menuOverlay.classList.remove('active');
            }
            document.body.classList.remove('no-scroll');
        }
    });
});