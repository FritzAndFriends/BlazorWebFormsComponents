// ModalPopupExtender JS behavior module
// Displays a target element as a modal popup with overlay backdrop,
// focus trapping, centering, drag support, and Escape key dismissal.

const behaviors = new Map();

/**
 * Creates a modal popup behavior and attaches it to the target trigger element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const trigger = document.getElementById(targetId);

    if (!trigger) {
        console.warn(`[ModalPopupExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const popup = document.getElementById(properties.popupControlId);
    if (!popup) {
        console.warn(`[ModalPopupExtender] Popup element '${properties.popupControlId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        popup,
        overlay: null,
        isOpen: false,
        dragState: null,
        handlers: {}
    };

    // Hide popup initially
    popup.style.display = "none";

    function show() {
        if (state.isOpen) return;
        state.isOpen = true;

        // Create overlay backdrop
        const overlay = document.createElement("div");
        overlay.style.cssText = "position:fixed;top:0;left:0;width:100%;height:100%;z-index:10000;";
        if (state.properties.backgroundCssClass) {
            overlay.className = state.properties.backgroundCssClass;
        } else {
            overlay.style.backgroundColor = "rgba(0,0,0,0.5)";
        }
        document.body.appendChild(overlay);
        state.overlay = overlay;

        // Show and position popup
        popup.style.display = "";
        popup.style.position = "fixed";
        popup.style.zIndex = "10001";
        centerPopup();

        if (state.properties.dropShadow) {
            popup.style.boxShadow = "4px 4px 10px rgba(0,0,0,0.4)";
        }

        // Set up drag if enabled
        if (state.properties.drag) {
            setupDrag();
        }

        // Bind OK/Cancel buttons
        bindActionButtons();

        // Focus trap and Escape key
        document.addEventListener("keydown", state.handlers.keydown);

        // Focus first focusable element in popup
        const focusable = popup.querySelectorAll(
            'a[href], button:not([disabled]), textarea, input:not([type="hidden"]), select, [tabindex]:not([tabindex="-1"])'
        );
        if (focusable.length > 0) {
            focusable[0].focus();
        }
    }

    function hide(isOk) {
        if (!state.isOpen) return;
        state.isOpen = false;

        // Run callback script
        const script = isOk ? state.properties.onOkScript : state.properties.onCancelScript;
        if (script) {
            try { new Function(script)(); } catch (e) { console.error("[ModalPopupExtender] Script error:", e); }
        }

        // Clean up drag
        teardownDrag();

        // Unbind action buttons
        unbindActionButtons();

        // Remove overlay
        if (state.overlay) {
            state.overlay.remove();
            state.overlay = null;
        }

        // Hide popup and reset positioning
        popup.style.display = "none";
        popup.style.boxShadow = "";

        document.removeEventListener("keydown", state.handlers.keydown);

        // Return focus to trigger
        trigger.focus();
    }

    function centerPopup() {
        const rect = popup.getBoundingClientRect();
        popup.style.left = `${Math.max(0, (window.innerWidth - rect.width) / 2)}px`;
        popup.style.top = `${Math.max(0, (window.innerHeight - rect.height) / 2)}px`;
    }

    function setupDrag() {
        const handleId = state.properties.popupDragHandleControlId;
        const handle = handleId ? document.getElementById(handleId) : popup;
        if (!handle) return;

        handle.style.cursor = "move";

        function onMouseDown(e) {
            if (e.button !== 0) return;
            e.preventDefault();
            const rect = popup.getBoundingClientRect();
            state.dragState = {
                startX: e.clientX,
                startY: e.clientY,
                origLeft: rect.left,
                origTop: rect.top
            };
            document.addEventListener("mousemove", onMouseMove);
            document.addEventListener("mouseup", onMouseUp);
        }

        function onMouseMove(e) {
            if (!state.dragState) return;
            const dx = e.clientX - state.dragState.startX;
            const dy = e.clientY - state.dragState.startY;
            popup.style.left = `${state.dragState.origLeft + dx}px`;
            popup.style.top = `${state.dragState.origTop + dy}px`;
        }

        function onMouseUp() {
            state.dragState = null;
            document.removeEventListener("mousemove", onMouseMove);
            document.removeEventListener("mouseup", onMouseUp);
        }

        handle.addEventListener("mousedown", onMouseDown);
        state.handlers.dragHandle = handle;
        state.handlers.dragMouseDown = onMouseDown;
        state.handlers.dragMouseMove = onMouseMove;
        state.handlers.dragMouseUp = onMouseUp;
    }

    function teardownDrag() {
        if (state.handlers.dragHandle) {
            state.handlers.dragHandle.removeEventListener("mousedown", state.handlers.dragMouseDown);
            state.handlers.dragHandle.style.cursor = "";
        }
        if (state.dragState) {
            document.removeEventListener("mousemove", state.handlers.dragMouseMove);
            document.removeEventListener("mouseup", state.handlers.dragMouseUp);
            state.dragState = null;
        }
    }

    function bindActionButtons() {
        if (state.properties.okControlId) {
            const okBtn = document.getElementById(state.properties.okControlId);
            if (okBtn) {
                state.handlers.okClick = () => hide(true);
                okBtn.addEventListener("click", state.handlers.okClick);
                state.handlers.okBtn = okBtn;
            }
        }
        if (state.properties.cancelControlId) {
            const cancelBtn = document.getElementById(state.properties.cancelControlId);
            if (cancelBtn) {
                state.handlers.cancelClick = () => hide(false);
                cancelBtn.addEventListener("click", state.handlers.cancelClick);
                state.handlers.cancelBtn = cancelBtn;
            }
        }
    }

    function unbindActionButtons() {
        if (state.handlers.okBtn) {
            state.handlers.okBtn.removeEventListener("click", state.handlers.okClick);
        }
        if (state.handlers.cancelBtn) {
            state.handlers.cancelBtn.removeEventListener("click", state.handlers.cancelClick);
        }
    }

    // Keydown handler for Escape and focus trapping
    state.handlers.keydown = function (e) {
        if (e.key === "Escape") {
            hide(false);
            return;
        }
        // Focus trap
        if (e.key === "Tab") {
            const focusable = popup.querySelectorAll(
                'a[href], button:not([disabled]), textarea, input:not([type="hidden"]), select, [tabindex]:not([tabindex="-1"])'
            );
            if (focusable.length === 0) return;
            const first = focusable[0];
            const last = focusable[focusable.length - 1];
            if (e.shiftKey) {
                if (document.activeElement === first) {
                    e.preventDefault();
                    last.focus();
                }
            } else {
                if (document.activeElement === last) {
                    e.preventDefault();
                    first.focus();
                }
            }
        }
    };

    // Trigger click opens the modal
    state.handlers.triggerClick = function (e) {
        e.preventDefault();
        show();
    };
    trigger.addEventListener("click", state.handlers.triggerClick);

    state.show = show;
    state.hide = hide;
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
    state.properties = { ...properties };
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Close if open
    if (state.isOpen) {
        state.hide(false);
    }

    // Remove trigger listener
    const trigger = document.getElementById(state.targetId);
    if (trigger && state.handlers.triggerClick) {
        trigger.removeEventListener("click", state.handlers.triggerClick);
    }

    behaviors.delete(behaviorId);
}
