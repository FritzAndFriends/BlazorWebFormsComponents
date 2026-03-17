// DragPanelExtender JS behavior module
// Makes a panel/element draggable by its title bar or handle.

const behaviors = new Map();

/**
 * Creates a drag behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[DragPanelExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const { dragHandleId } = properties;

    // Determine the drag handle: use specified element or the target itself
    const dragHandle = dragHandleId
        ? document.getElementById(dragHandleId)
        : target;

    if (!dragHandle) {
        console.warn(`[DragPanelExtender] Drag handle '${dragHandleId}' not found.`);
        return {};
    }

    // Track drag state
    let isDragging = false;
    let offsetX = 0;
    let offsetY = 0;

    // Ensure target is positioned for dragging
    const computedStyle = window.getComputedStyle(target);
    if (computedStyle.position === "static") {
        target.style.position = "relative";
    }

    /**
     * Handles mousedown on the drag handle to start dragging.
     * @param {MouseEvent} e
     */
    function mouseDownHandler(e) {
        // Only respond to left mouse button
        if (e.button !== 0) return;

        isDragging = true;

        // Calculate offset from mouse position to element position
        const targetRect = target.getBoundingClientRect();
        offsetX = e.clientX - targetRect.left;
        offsetY = e.clientY - targetRect.top;

        // Prevent text selection during drag
        e.preventDefault();

        // Set cursor for dragging feedback
        document.body.style.cursor = "move";
        dragHandle.style.cursor = "move";

        // Add document-level listeners for move and up
        document.addEventListener("mousemove", mouseMoveHandler);
        document.addEventListener("mouseup", mouseUpHandler);
    }

    /**
     * Handles mousemove to update element position while dragging.
     * @param {MouseEvent} e
     */
    function mouseMoveHandler(e) {
        if (!isDragging) return;

        // Calculate new position
        const newLeft = e.clientX - offsetX;
        const newTop = e.clientY - offsetY;

        // Get parent's position for relative positioning
        const parent = target.offsetParent || document.body;
        const parentRect = parent.getBoundingClientRect();

        // Update element position relative to its offset parent
        target.style.left = (newLeft - parentRect.left) + "px";
        target.style.top = (newTop - parentRect.top) + "px";
    }

    /**
     * Handles mouseup to stop dragging.
     * @param {MouseEvent} e
     */
    function mouseUpHandler(e) {
        if (!isDragging) return;

        isDragging = false;

        // Reset cursors
        document.body.style.cursor = "";
        dragHandle.style.cursor = "move";

        // Remove document-level listeners
        document.removeEventListener("mousemove", mouseMoveHandler);
        document.removeEventListener("mouseup", mouseUpHandler);
    }

    /**
     * Handles selectstart to prevent text selection during drag.
     * @param {Event} e
     */
    function selectStartHandler(e) {
        if (isDragging) {
            e.preventDefault();
        }
    }

    // Set cursor to indicate draggable area
    dragHandle.style.cursor = "move";

    // Attach event listeners
    dragHandle.addEventListener("mousedown", mouseDownHandler);
    document.addEventListener("selectstart", selectStartHandler);

    const state = {
        targetId,
        dragHandleId,
        dragHandle,
        isDragging,
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

    const { dragHandleId } = properties;

    // If drag handle changed, we need to rebind events
    if (dragHandleId !== state.dragHandleId) {
        const target = document.getElementById(state.targetId);
        if (!target) return;

        // Remove old handler
        if (state.dragHandle) {
            state.dragHandle.removeEventListener("mousedown", state.mouseDownHandler);
            state.dragHandle.style.cursor = "";
        }

        // Get new handle
        const newHandle = dragHandleId
            ? document.getElementById(dragHandleId)
            : target;

        if (newHandle) {
            newHandle.addEventListener("mousedown", state.mouseDownHandler);
            newHandle.style.cursor = "move";
            state.dragHandle = newHandle;
            state.dragHandleId = dragHandleId;
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

    // Remove drag handle listener
    if (state.dragHandle) {
        state.dragHandle.removeEventListener("mousedown", state.mouseDownHandler);
        state.dragHandle.style.cursor = "";
    }

    // Remove document-level listeners
    document.removeEventListener("mousemove", state.mouseMoveHandler);
    document.removeEventListener("mouseup", state.mouseUpHandler);
    document.removeEventListener("selectstart", state.selectStartHandler);

    behaviors.delete(behaviorId);
}
