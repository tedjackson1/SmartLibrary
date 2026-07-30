"use strict";

/*
 * =========================================================
 * LibSphere Library Management System
 * Global user-interface interactions
 * File: wwwroot/js/site.js
 * =========================================================
 *
 * This file controls presentation-only interactions:
 * - Page loader
 * - Responsive sidebar
 * - Light and dark themes
 * - Scroll-to-top button
 * - Bootstrap tooltips
 * - Password visibility buttons when present
 *
 * It does not modify application logic, routes, form actions,
 * model binding, API calls, database operations or data flow.
 * =========================================================
 */

(function () {
    const MOBILE_BREAKPOINT = 991.98;
    const SCROLL_BUTTON_THRESHOLD = 320;
    const THEME_STORAGE_KEY = "libsphere-theme";

    const documentElement = document.documentElement;

    documentElement.classList.add("js-enabled");

    /**
     * Safely reads a value from local storage.
     *
     * @param {string} key
     * @returns {string|null}
     */
    function getStoredValue(key) {
        try {
            return window.localStorage.getItem(key);
        } catch (error) {
            return null;
        }
    }

    /**
     * Safely saves a value in local storage.
     *
     * @param {string} key
     * @param {string} value
     */
    function setStoredValue(key, value) {
        try {
            window.localStorage.setItem(key, value);
        } catch (error) {
            /*
             * Local storage can be unavailable in private browsing
             * or when storage permissions are blocked. The theme
             * will still work for the current page.
             */
        }
    }

    /**
     * Returns true when the browser is displaying the mobile layout.
     *
     * @returns {boolean}
     */
    function isMobileLayout() {
        return window.innerWidth <= MOBILE_BREAKPOINT;
    }

    /**
     * Determines the initial colour theme.
     *
     * @returns {"light"|"dark"}
     */
    function getInitialTheme() {
        const storedTheme = getStoredValue(THEME_STORAGE_KEY);

        if (storedTheme === "light" || storedTheme === "dark") {
            return storedTheme;
        }

        if (
            window.matchMedia &&
            window.matchMedia("(prefers-color-scheme: dark)").matches
        ) {
            return "dark";
        }

        return "light";
    }

    /**
     * Updates the theme button icon and accessible text.
     *
     * @param {"light"|"dark"} theme
     */
    function updateThemeButton(theme) {
        const themeToggleButton =
            document.getElementById("themeToggleButton");

        const themeToggleIcon =
            document.getElementById("themeToggleIcon");

        if (!themeToggleButton || !themeToggleIcon) {
            return;
        }

        const darkThemeIsActive = theme === "dark";

        themeToggleIcon.className = darkThemeIsActive
            ? "bi bi-sun"
            : "bi bi-moon-stars";

        const buttonLabel = darkThemeIsActive
            ? "Switch to light theme"
            : "Switch to dark theme";

        themeToggleButton.setAttribute("aria-label", buttonLabel);
        themeToggleButton.setAttribute("title", buttonLabel);
    }

    /**
     * Applies the requested colour theme.
     *
     * @param {"light"|"dark"} theme
     * @param {boolean} savePreference
     */
    function applyTheme(theme, savePreference) {
        const validTheme = theme === "dark"
            ? "dark"
            : "light";

        documentElement.setAttribute(
            "data-theme",
            validTheme
        );

        updateThemeButton(validTheme);

        if (savePreference) {
            setStoredValue(
                THEME_STORAGE_KEY,
                validTheme
            );
        }
    }

    /**
     * Sets up the colour-theme button.
     */
    function initializeThemeToggle() {
        const themeToggleButton =
            document.getElementById("themeToggleButton");

        const currentTheme =
            documentElement.getAttribute("data-theme") ||
            getInitialTheme();

        applyTheme(currentTheme, false);

        if (!themeToggleButton) {
            return;
        }

        themeToggleButton.addEventListener("click", function () {
            const activeTheme =
                documentElement.getAttribute("data-theme");

            const nextTheme = activeTheme === "dark"
                ? "light"
                : "dark";

            applyTheme(nextTheme, true);
        });
    }

    /**
     * Sets up the responsive sidebar.
     */
    function initializeSidebar() {
        const sidebar =
            document.getElementById("appSidebar");

        const sidebarToggleButton =
            document.getElementById("sidebarToggleButton");

        const sidebarCloseButton =
            document.getElementById("sidebarCloseButton");

        const sidebarBackdrop =
            document.getElementById("sidebarBackdrop");

        if (!sidebar) {
            return;
        }

        /**
         * Opens the sidebar on tablet and mobile screens.
         */
        function openSidebar() {
            if (!isMobileLayout()) {
                return;
            }

            sidebar.classList.add("is-open");
            document.body.classList.add("sidebar-open");

            if (sidebarBackdrop) {
                sidebarBackdrop.classList.add("is-visible");
            }

            if (sidebarToggleButton) {
                sidebarToggleButton.setAttribute(
                    "aria-expanded",
                    "true"
                );
            }

            sidebar.setAttribute("aria-hidden", "false");
        }

        /**
         * Closes the responsive sidebar.
         */
        function closeSidebar() {
            sidebar.classList.remove("is-open");
            document.body.classList.remove("sidebar-open");

            if (sidebarBackdrop) {
                sidebarBackdrop.classList.remove("is-visible");
            }

            if (sidebarToggleButton) {
                sidebarToggleButton.setAttribute(
                    "aria-expanded",
                    "false"
                );
            }

            if (isMobileLayout()) {
                sidebar.setAttribute("aria-hidden", "true");
            } else {
                sidebar.removeAttribute("aria-hidden");
            }
        }

        /**
         * Synchronises accessibility attributes after resizing.
         */
        function synchronizeSidebarState() {
            if (isMobileLayout()) {
                const sidebarIsOpen =
                    sidebar.classList.contains("is-open");

                sidebar.setAttribute(
                    "aria-hidden",
                    sidebarIsOpen ? "false" : "true"
                );
            } else {
                closeSidebar();
                sidebar.removeAttribute("aria-hidden");
            }
        }

        if (sidebarToggleButton) {
            sidebarToggleButton.setAttribute(
                "aria-controls",
                "appSidebar"
            );

            sidebarToggleButton.setAttribute(
                "aria-expanded",
                "false"
            );

            sidebarToggleButton.addEventListener(
                "click",
                function () {
                    const sidebarIsOpen =
                        sidebar.classList.contains("is-open");

                    if (sidebarIsOpen) {
                        closeSidebar();
                    } else {
                        openSidebar();
                    }
                }
            );
        }

        if (sidebarCloseButton) {
            sidebarCloseButton.addEventListener(
                "click",
                closeSidebar
            );
        }

        if (sidebarBackdrop) {
            sidebarBackdrop.addEventListener(
                "click",
                closeSidebar
            );
        }

        sidebar
            .querySelectorAll("a.sidebar-link, a.sidebar-login-link")
            .forEach(function (navigationLink) {
                navigationLink.addEventListener(
                    "click",
                    function () {
                        if (isMobileLayout()) {
                            closeSidebar();
                        }
                    }
                );
            });

        document.addEventListener(
            "keydown",
            function (event) {
                if (
                    event.key === "Escape" &&
                    sidebar.classList.contains("is-open")
                ) {
                    closeSidebar();

                    if (sidebarToggleButton) {
                        sidebarToggleButton.focus();
                    }
                }
            }
        );

        let resizeTimeoutId = null;

        window.addEventListener(
            "resize",
            function () {
                window.clearTimeout(resizeTimeoutId);

                resizeTimeoutId = window.setTimeout(
                    synchronizeSidebarState,
                    100
                );
            }
        );

        synchronizeSidebarState();
    }

    /**
     * Hides the initial page-loading screen.
     */
    function initializePageLoader() {
        const pageLoader =
            document.getElementById("pageLoader");

        if (!pageLoader) {
            return;
        }

        let loaderHasBeenHidden = false;

        /**
         * Hides the loader once without removing it from the DOM.
         */
        function hidePageLoader() {
            if (loaderHasBeenHidden) {
                return;
            }

            loaderHasBeenHidden = true;
            pageLoader.classList.add("is-hidden");
            pageLoader.setAttribute("aria-hidden", "true");

            window.setTimeout(function () {
                pageLoader.style.display = "none";
            }, 350);
        }

        if (document.readyState === "complete") {
            window.setTimeout(hidePageLoader, 120);
        } else {
            window.addEventListener(
                "load",
                function () {
                    window.setTimeout(
                        hidePageLoader,
                        120
                    );
                },
                { once: true }
            );
        }

        /*
         * Safety fallback so that a failed image or external resource
         * cannot leave the page loader visible indefinitely.
         */
        window.setTimeout(
            hidePageLoader,
            3000
        );
    }

    /**
     * Sets up the floating scroll-to-top button.
     */
    function initializeScrollToTopButton() {
        const scrollToTopButton =
            document.getElementById("scrollToTopButton");

        if (!scrollToTopButton) {
            return;
        }

        /**
         * Shows or hides the button according to scroll position.
         */
        function updateScrollButtonVisibility() {
            if (window.scrollY > SCROLL_BUTTON_THRESHOLD) {
                scrollToTopButton.classList.add("is-visible");
            } else {
                scrollToTopButton.classList.remove("is-visible");
            }
        }

        scrollToTopButton.addEventListener(
            "click",
            function () {
                const reducedMotionIsPreferred =
                    window.matchMedia &&
                    window.matchMedia(
                        "(prefers-reduced-motion: reduce)"
                    ).matches;

                window.scrollTo({
                    top: 0,
                    behavior: reducedMotionIsPreferred
                        ? "auto"
                        : "smooth"
                });
            }
        );

        window.addEventListener(
            "scroll",
            updateScrollButtonVisibility,
            { passive: true }
        );

        updateScrollButtonVisibility();
    }

    /**
     * Activates Bootstrap tooltips when tooltip elements exist.
     */
    function initializeBootstrapTooltips() {
        if (
            typeof window.bootstrap === "undefined" ||
            typeof window.bootstrap.Tooltip === "undefined"
        ) {
            return;
        }

        const tooltipElements =
            document.querySelectorAll(
                '[data-bs-toggle="tooltip"]'
            );

        tooltipElements.forEach(function (element) {
            window.bootstrap.Tooltip.getOrCreateInstance(
                element
            );
        });
    }

    /**
     * Enables password visibility buttons when a page contains them.
     *
     * Supported markup:
     * <button
     *     type="button"
     *     data-password-toggle
     *     data-password-target="PasswordInputId">
     * </button>
     */
    function initializePasswordToggles() {
        const passwordToggleButtons =
            document.querySelectorAll(
                "[data-password-toggle]"
            );

        passwordToggleButtons.forEach(
            function (toggleButton) {
                const targetId =
                    toggleButton.getAttribute(
                        "data-password-target"
                    );

                if (!targetId) {
                    return;
                }

                const passwordInput =
                    document.getElementById(targetId);

                if (
                    !passwordInput ||
                    passwordInput.tagName !== "INPUT"
                ) {
                    return;
                }

                toggleButton.addEventListener(
                    "click",
                    function () {
                        const passwordIsVisible =
                            passwordInput.type === "text";

                        passwordInput.type = passwordIsVisible
                            ? "password"
                            : "text";

                        const toggleIcon =
                            toggleButton.querySelector("i");

                        if (toggleIcon) {
                            toggleIcon.className =
                                passwordIsVisible
                                    ? "bi bi-eye"
                                    : "bi bi-eye-slash";
                        }

                        const buttonLabel =
                            passwordIsVisible
                                ? "Show password"
                                : "Hide password";

                        toggleButton.setAttribute(
                            "aria-label",
                            buttonLabel
                        );

                        toggleButton.setAttribute(
                            "title",
                            buttonLabel
                        );
                    }
                );
            }
        );
    }

    /**
     * Prevents Bootstrap dropdown menus from extending outside
     * very narrow mobile screens.
     */
    function initializeDropdownAccessibility() {
        const dropdownButtons =
            document.querySelectorAll(
                '[data-bs-toggle="dropdown"]'
            );

        dropdownButtons.forEach(
            function (dropdownButton) {
                if (
                    !dropdownButton.hasAttribute(
                        "aria-haspopup"
                    )
                ) {
                    dropdownButton.setAttribute(
                        "aria-haspopup",
                        "true"
                    );
                }
            }
        );
    }

    /**
     * Starts all presentation-only interface interactions.
     */
    function initializeInterface() {
        initializeThemeToggle();
        initializeSidebar();
        initializePageLoader();
        initializeScrollToTopButton();
        initializeBootstrapTooltips();
        initializePasswordToggles();
        initializeDropdownAccessibility();
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initializeInterface,
            { once: true }
        );
    } else {
        initializeInterface();
    }
})();