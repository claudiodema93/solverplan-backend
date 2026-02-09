// FSH Responsive Utilities
// Provides viewport detection and responsive behavior helpers

window.FshResponsive = {
    // Mobile breakpoint (matches MudBlazor's 'sm' breakpoint)
    MOBILE_BREAKPOINT: 768,

    // Current viewport state
    isMobile: false,
    listeners: [],

    /**
     * Initialize responsive detection
     */
    initialize: function () {
        this.updateViewport();
        this.setupResizeListener();
        console.log('[FshResponsive] Responsive detection initialized');
    },

    /**
     * Check if current viewport is mobile
     * @returns {boolean}
     */
    checkIsMobile: function () {
        return window.innerWidth < this.MOBILE_BREAKPOINT;
    },

    /**
     * Update viewport state
     */
    updateViewport: function () {
        const wasMobile = this.isMobile;
        this.isMobile = this.checkIsMobile();

        // Notify listeners if state changed
        if (wasMobile !== this.isMobile) {
            this.notifyListeners();
        }
    },

    /**
     * Setup window resize listener with debounce
     */
    setupResizeListener: function () {
        let resizeTimeout;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                this.updateViewport();
            }, 150); // Debounce resize events
        });
    },

    /**
     * Register a listener for viewport changes
     * @param {any} dotNetHelper - .NET object reference
     * @param {string} methodName - Method name to invoke
     */
    addListener: function (dotNetHelper, methodName) {
        this.listeners.push({ dotNetHelper, methodName });

        // Immediately notify the new listener of current state
        try {
            dotNetHelper.invokeMethodAsync(methodName, this.isMobile);
        } catch (error) {
            console.error('[FshResponsive] Failed to notify listener:', error);
        }
    },

    /**
     * Remove a listener
     * @param {any} dotNetHelper - .NET object reference
     */
    removeListener: function (dotNetHelper) {
        this.listeners = this.listeners.filter(l => l.dotNetHelper !== dotNetHelper);
    },

    /**
     * Notify all registered listeners of viewport change
     */
    notifyListeners: function () {
        console.log(`[FshResponsive] Viewport changed: ${this.isMobile ? 'Mobile' : 'Desktop'}`);

        this.listeners.forEach(({ dotNetHelper, methodName }) => {
            try {
                dotNetHelper.invokeMethodAsync(methodName, this.isMobile);
            } catch (error) {
                console.error('[FshResponsive] Failed to notify listener:', error);
            }
        });
    },

    /**
     * Get current viewport width
     * @returns {number}
     */
    getViewportWidth: function () {
        return window.innerWidth;
    },

    /**
     * Get current viewport height
     * @returns {number}
     */
    getViewportHeight: function () {
        return window.innerHeight;
    },

    /**
     * Check if device is in portrait orientation
     * @returns {boolean}
     */
    isPortrait: function () {
        return window.innerHeight > window.innerWidth;
    },

    /**
     * Check if device is in landscape orientation
     * @returns {boolean}
     */
    isLandscape: function () {
        return window.innerWidth > window.innerHeight;
    }
};

// Auto-initialize on page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.FshResponsive.initialize();
    });
} else {
    window.FshResponsive.initialize();
}
