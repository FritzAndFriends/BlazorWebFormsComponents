// ResizableControlExtender JS behavior module
// Allows users to resize an element by dragging its edges or a resize handle.

const behaviors = new Map();

/**
 * Creates a resizable behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[ResizableControlExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const {
        handleCssClass,
        resizableCssClass,
        minimumWidth,
        minimumHeight,
        maximumWidth,
        maximumHeight
    } = properties;

    // Track resize state
    let isResizing = false;
    let startX = 0;
    let startY = 0;
    let startWidth = 0;
    let startHeight = 0;

    // Ensure target is positioned for resizing
    const computedStyle = window.getComputedStyle(target);
    if (computedStyle.position === "static") {
        target.style.position = "relative";
    }

    // Create or find the resize handle
    let resizeHandle = null;

    if (handleCssClass) {
        // Look for an existing element with the handle CSS class inside the target
        resizeHandle = target.querySelector("." + handleCssClass);
    }

    if (!resizeHandle) {
        // Create a default resize handle at the bottom-right corner
        resizeHandle = document.createElement("div");
        resizeHandle.style.position = "absolute";
        resizeHandle.style.right = "0";
        resizeHandle.style.bottom = "0";
        resizeHandle.style.width = "16px";
        resizeHandle.style.height = "16px";
        resizeHandle.style.cursor = "se-resize";
        resizeHandle.style.background = "linear-gradient(135deg, transparent 50%, #999 50%)";

        if (handleCssClass) {
            resizeHandle.className = handleCssClass;
        }

        target.appendChild(resizeHandle);
    } else {
        // Ensure the existing handle has the resize cursor
        resizeHandle.style.cursor = "se-resize";
    }

    /**
     * Clamps a value between min and max constraints.
     * @param {number} value - The value to clamp
     * @param {number} min - Minimum constraint (0 = no min)
     * @param {number} max - Maximum constraint (0 = no max)
     * @returns {number} The clamped value
     */
    function clamp(value, min, max) {
        if (min > 0 && value < min) return min;
        if (max > 0 && value > max) return max;
        return value;
    }

    /**
     * Handles mousedown on the resize handle to start resizing.
     * @param {MouseEvent} e
     */
    function mouseDownHandler(e) {
        // Only respond to left mouse button
        if (e.button !== 0) return;

        isResizing = true;

        // Record starting positions
        startX = e.clientX;
        startY = e.clientY;
        startWidth = target.offsetWidth;
        startHeight = target.offsetHeight;

        // Prevent text selection during resize
        e.preventDefault();
        e.stopPropagation();

        // Apply resizing CSS class
        if (resizableCssClass) {
            target.classList.add(resizableCssClass);
        }

        // Set cursor for resizing feedback
        document.body.style.cursor = "se-resize";

        // Add document-level listeners for move and up
        document.addEventListener("mousemove", mouseMoveHandler);
        document.addEventListener("mouseup", mouseUpHandler);
    }

    /**
     * Handles mousemove to update element size while resizing.
     * @param {MouseEvent} e
     */
    function mouseMoveHandler(e) {
        if (!isResizing) return;

        // Calculate the change in mouse position
        const deltaX = e.clientX - startX;
        const deltaY = e.clientY - startY;

        // Calculate new dimensions
        let newWidth = startWidth + deltaX;
        let newHeight = startHeight + deltaY;

        // Apply constraints
        newWidth = clamp(newWidth, minimumWidth, maximumWidth);
        newHeight = clamp(newHeight, minimumHeight, maximumHeight);

        // Update element size
        target.style.width = newWidth + "px";
        target.style.height = newHeight + "px";
    }

    /**
     * Handles mouseup to stop resizing.
     * @param {MouseEvent} e
     */
    function mouseUpHandler(e) {
        if (!isResizing) return;

        isResizing = false;

        // Remove resizing CSS class
        if (resizableCssClass) {
            target.classList.remove(resizableCssClass);
        }

        // Reset cursor
        document.body.style.cursor = "";

        // Remove document-level listeners
        document.removeEventListener("mousemove", mouseMoveHandler);
        document.removeEventListener("mouseup", mouseUpHandler);
    }

    /**
     * Handles selectstart to prevent text selection during resize.
     * @param {Event} e
     */
    function selectStartHandler(e) {
        if (isResizing) {
            e.preventDefault();
        }
    }

    // Attach event listeners
    resizeHandle.addEventListener("mousedown", mouseDownHandler);
    document.addEventListener("selectstart", selectStartHandler);

    const state = {
        targetId,
        target,
        resizeHandle,
        handleCssClass,
        resizableCssClass,
        minimumWidth,
        minimumHeight,
        maximumWidth,
        maximumHeight,
        handleCreatedByUs: !handleCssClass || !target.querySelector("." + handleCssClass),
        mouseDownHandler,
        mouseMoveHandler,
        mouseUpHandler,
        selectStartHandler
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

    const {
        handleCssClass,
        resizableCssClass,
        minimumWidth,
        minimumHeight,
        maximumWidth,
        maximumHeight
    } = properties;

    // Update constraint values
    state.minimumWidth = minimumWidth;
    state.minimumHeight = minimumHeight;
    state.maximumWidth = maximumWidth;
    state.maximumHeight = maximumHeight;
    state.resizableCssClass = resizableCssClass;

    // If handle CSS class changed, we need to update the handle
    if (handleCssClass !== state.handleCssClass) {
        // Remove old handle class and add new one if applicable
        if (state.handleCssClass && state.resizeHandle) {
            state.resizeHandle.classList.remove(state.handleCssClass);
        }
        if (handleCssClass && state.resizeHandle) {
            state.resizeHandle.classList.add(handleCssClass);
        }
        state.handleCssClass = handleCssClass;
    }
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Remove resize handle listener
    if (state.resizeHandle) {
        state.resizeHandle.removeEventListener("mousedown", state.mouseDownHandler);

        // Remove the handle element if we created it
        if (state.handleCreatedByUs && state.resizeHandle.parentNode) {
            state.resizeHandle.parentNode.removeChild(state.resizeHandle);
        }
    }

    // Remove document-level listeners
    document.removeEventListener("mousemove", state.mouseMoveHandler);
    document.removeEventListener("mouseup", state.mouseUpHandler);
    document.removeEventListener("selectstart", state.selectStartHandler);

    behaviors.delete(behaviorId);
}
