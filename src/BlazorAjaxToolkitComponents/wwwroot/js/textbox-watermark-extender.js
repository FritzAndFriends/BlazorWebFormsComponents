// TextBoxWatermarkExtender JS behavior module
// Displays placeholder/watermark text in a target text field when it is empty.

const behaviors = new Map();

/**
 * Creates a watermark behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[TextBoxWatermarkExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const { watermarkText, watermarkCssClass } = properties;

    // Track state
    let isShowingWatermark = false;
    let originalCssClass = target.className;

    /**
     * Shows the watermark in the text field.
     */
    function showWatermark() {
        if (isShowingWatermark) return;
        
        target.value = watermarkText;
        isShowingWatermark = true;
        
        if (watermarkCssClass) {
            target.className = originalCssClass 
                ? originalCssClass + " " + watermarkCssClass 
                : watermarkCssClass;
        }
    }

    /**
     * Hides the watermark and restores the original state.
     */
    function hideWatermark() {
        if (!isShowingWatermark) return;
        
        target.value = "";
        isShowingWatermark = false;
        
        target.className = originalCssClass;
    }

    /**
     * Focus handler: hide watermark when user focuses the field.
     */
    function focusHandler() {
        if (isShowingWatermark) {
            hideWatermark();
        }
    }

    /**
     * Blur handler: show watermark if field is empty when user leaves.
     */
    function blurHandler() {
        if (target.value === "" || target.value === watermarkText) {
            showWatermark();
        }
    }

    /**
     * Input handler: track when user has typed something.
     */
    function inputHandler() {
        // If user is typing, we're not showing watermark anymore
        if (isShowingWatermark && target.value !== watermarkText) {
            isShowingWatermark = false;
            target.className = originalCssClass;
        }
    }

    // Attach event listeners
    target.addEventListener("focus", focusHandler);
    target.addEventListener("blur", blurHandler);
    target.addEventListener("input", inputHandler);

    // Initialize: show watermark if field is empty
    if (target.value === "") {
        showWatermark();
    }

    const state = {
        targetId,
        watermarkText,
        watermarkCssClass,
        originalCssClass,
        isShowingWatermark,
        focusHandler,
        blurHandler,
        inputHandler,
        showWatermark,
        hideWatermark
    };

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);
    if (!target) return;

    const { watermarkText, watermarkCssClass } = properties;
    const wasShowingWatermark = state.isShowingWatermark;

    // Update stored properties
    state.watermarkText = watermarkText;
    state.watermarkCssClass = watermarkCssClass;

    // If currently showing watermark, update the displayed text
    if (wasShowingWatermark) {
        target.value = watermarkText;
        
        // Update CSS class
        target.className = state.originalCssClass;
        if (watermarkCssClass) {
            target.className = state.originalCssClass 
                ? state.originalCssClass + " " + watermarkCssClass 
                : watermarkCssClass;
        }
    }
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);
    if (target) {
        target.removeEventListener("focus", state.focusHandler);
        target.removeEventListener("blur", state.blurHandler);
        target.removeEventListener("input", state.inputHandler);

        // Restore original state if showing watermark
        if (state.isShowingWatermark) {
            target.value = "";
            target.className = state.originalCssClass;
        }
    }

    behaviors.delete(behaviorId);
}
