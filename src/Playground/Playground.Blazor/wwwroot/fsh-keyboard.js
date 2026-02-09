// FSH Keyboard Navigation and Shortcuts
// Provides keyboard shortcuts and accessibility improvements

window.FshKeyboard = {
    // Track active shortcuts and their callbacks
    shortcuts: new Map(),
    isInitialized: false,

    /**
     * Initialize keyboard navigation system
     * @param {any} dotNetHelper - .NET object reference for callbacks
     */
    initialize: function (dotNetHelper) {
        if (this.isInitialized) {
            return;
        }

        this.dotNetHelper = dotNetHelper;
        this.setupGlobalKeyboardHandler();
        this.setupFocusTrap();
        this.isInitialized = true;

        console.log('[FshKeyboard] Keyboard navigation initialized');
    },

    /**
     * Setup global keyboard event handler
     */
    setupGlobalKeyboardHandler: function () {
        document.addEventListener('keydown', (e) => {
            // Skip if user is typing in an input field
            const activeElement = document.activeElement;
            const isInputActive = activeElement &&
                (activeElement.tagName === 'INPUT' ||
                    activeElement.tagName === 'TEXTAREA' ||
                    activeElement.isContentEditable);

            // Handle '/' key to focus search (unless in input)
            if (e.key === '/' && !isInputActive && !e.ctrlKey && !e.metaKey && !e.altKey) {
                e.preventDefault();
                this.focusSearch();
                return;
            }

            // Handle Ctrl/Cmd shortcuts (work anywhere)
            const modifier = e.ctrlKey || e.metaKey;

            if (modifier) {
                switch (e.key.toLowerCase()) {
                    case 'n':
                        // Ctrl+N / Cmd+N: New item (context-aware)
                        e.preventDefault();
                        this.invokeShortcut('new');
                        break;
                    case 'r':
                        // Ctrl+R / Cmd+R: Refresh (but allow browser refresh on forms)
                        if (!isInputActive) {
                            e.preventDefault();
                            this.invokeShortcut('refresh');
                        }
                        break;
                    case 's':
                        // Ctrl+S / Cmd+S: Save
                        e.preventDefault();
                        this.invokeShortcut('save');
                        break;
                    case 'k':
                        // Ctrl+K / Cmd+K: Command palette / quick search
                        e.preventDefault();
                        this.invokeShortcut('command-palette');
                        break;
                }
            }

            // Handle Escape key
            if (e.key === 'Escape') {
                this.handleEscape();
            }
        });
    },

    /**
     * Setup focus trap for improved keyboard navigation
     */
    setupFocusTrap: function () {
        // Add skip link functionality
        const skipLink = document.querySelector('.fsh-skip-link');
        if (skipLink) {
            skipLink.addEventListener('click', (e) => {
                e.preventDefault();
                const mainContent = document.querySelector('#main-content, main, [role="main"]');
                if (mainContent) {
                    mainContent.setAttribute('tabindex', '-1');
                    mainContent.focus();
                }
            });
        }
    },

    /**
     * Focus the search input
     */
    focusSearch: function () {
        // Try common search input selectors
        const searchInput = document.querySelector(
            'input[placeholder*="Search" i], input[placeholder*="search" i], input[type="search"], .fsh-search-input'
        );

        if (searchInput) {
            searchInput.focus();
            searchInput.select(); // Select existing text for easy replacement
            console.log('[FshKeyboard] Search input focused');
        } else {
            console.log('[FshKeyboard] Search input not found');
        }
    },

    /**
     * Invoke a registered shortcut
     * @param {string} action - The action name (e.g., 'new', 'refresh', 'save')
     */
    invokeShortcut: function (action) {
        if (this.dotNetHelper) {
            try {
                this.dotNetHelper.invokeMethodAsync('OnKeyboardShortcut', action);
                console.log(`[FshKeyboard] Shortcut invoked: ${action}`);
            } catch (error) {
                console.warn(`[FshKeyboard] Failed to invoke shortcut '${action}':`, error);
            }
        }
    },

    /**
     * Handle Escape key press
     */
    handleEscape: function () {
        // Close any open dialogs/modals
        const closeButtons = document.querySelectorAll(
            '.mud-dialog-close, .mud-overlay-dialog .mud-button-root[aria-label*="close" i]'
        );

        if (closeButtons.length > 0) {
            closeButtons[0].click();
            return;
        }

        // Clear search if focused
        const searchInput = document.activeElement;
        if (searchInput && searchInput.matches('input[type="search"], input[placeholder*="search" i]')) {
            searchInput.value = '';
            searchInput.dispatchEvent(new Event('input', { bubbles: true }));
            searchInput.blur();
        }
    },

    /**
     * Register a custom keyboard shortcut
     * @param {string} key - The key combination (e.g., 'ctrl+n', 'cmd+k')
     * @param {Function} callback - The callback function
     */
    registerShortcut: function (key, callback) {
        this.shortcuts.set(key.toLowerCase(), callback);
        console.log(`[FshKeyboard] Registered shortcut: ${key}`);
    },

    /**
     * Unregister a keyboard shortcut
     * @param {string} key - The key combination
     */
    unregisterShortcut: function (key) {
        this.shortcuts.delete(key.toLowerCase());
        console.log(`[FshKeyboard] Unregistered shortcut: ${key}`);
    },

    /**
     * Dispose and cleanup
     */
    dispose: function () {
        this.shortcuts.clear();
        this.dotNetHelper = null;
        this.isInitialized = false;
        console.log('[FshKeyboard] Keyboard navigation disposed');
    }
};

// Auto-initialize on page load (without dotnet helper for basic functionality)
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        // Basic initialization without .NET helper
        if (!window.FshKeyboard.isInitialized) {
            window.FshKeyboard.setupGlobalKeyboardHandler();
        }
    });
} else {
    if (!window.FshKeyboard.isInitialized) {
        window.FshKeyboard.setupGlobalKeyboardHandler();
    }
}
