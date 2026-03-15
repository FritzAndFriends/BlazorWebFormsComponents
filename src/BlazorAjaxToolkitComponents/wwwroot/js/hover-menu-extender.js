// HoverMenuExtender JS behavior module
// Displays a popup panel when the user hovers over a target control.
// Supports configurable show/hide delays, positional placement, and hover CSS styling.

const behaviors = new Map();

// PopupPosition (matches C# enum)
const POSITION_LEFT = 0;
const POSITION_RIGHT = 1;
const POSITION_TOP = 2;
const POSITION_BOTTOM = 3;
const POSITION_CENTER = 4;

/**
 * Creates a hover menu behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[HoverMenuExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const popup = document.getElementById(properties.popupControlId);
    if (!popup) {
        console.warn(`[HoverMenuExtender] Popup element '${properties.popupControlId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        popup,
        isVisible: false,
        showTimer: null,
        hideTimer: null,
        handlers: {}
    };

    // Hide popup initially
    popup.style.display = "none";

    function positionPopup() {
        const targetRect = target.getBoundingClientRect();
        const popupRect = popup.getBoundingClientRect();
        const scrollX = window.scrollX || window.pageXOffset;
        const scrollY = window.scrollY || window.pageYOffset;
        const offsetX = properties.offsetX || 0;
        const offsetY = properties.offsetY || 0;

        let left, top;

        switch (properties.popupPosition) {
            case POSITION_LEFT:
                left = targetRect.left - popupRect.width + scrollX + offsetX;
                top = targetRect.top + scrollY + offsetY;
                break;
            case POSITION_TOP:
                left = targetRect.left + scrollX + offsetX;
                top = targetRect.top - popupRect.height + scrollY + offsetY;
                break;
            case POSITION_BOTTOM:
                left = targetRect.left + scrollX + offsetX;
                top = targetRect.bottom + scrollY + offsetY;
                break;
            case POSITION_CENTER:
                left = targetRect.left + (targetRect.width - popupRect.width) / 2 + scrollX + offsetX;
                top = targetRect.top + (targetRect.height - popupRect.height) / 2 + scrollY + offsetY;
                break;
            case POSITION_RIGHT:
            default:
                left = targetRect.right + scrollX + offsetX;
                top = targetRect.top + scrollY + offsetY;
                break;
        }

        popup.style.left = `${Math.max(0, left)}px`;
        popup.style.top = `${Math.max(0, top)}px`;
    }

    function showPopup() {
        if (state.isVisible) return;
        state.isVisible = true;

        popup.style.display = "";
        popup.style.position = "absolute";
        popup.style.zIndex = "9999";
        positionPopup();

        if (properties.hoverCssClass) {
            target.classList.add(...properties.hoverCssClass.split(/\s+/).filter(Boolean));
        }
    }

    function hidePopup() {
        if (!state.isVisible) return;
        state.isVisible = false;

        popup.style.display = "none";

        if (properties.hoverCssClass) {
            target.classList.remove(...properties.hoverCssClass.split(/\s+/).filter(Boolean));
        }
    }

    function cancelTimers() {
        if (state.showTimer) {
            clearTimeout(state.showTimer);
            state.showTimer = null;
        }
        if (state.hideTimer) {
            clearTimeout(state.hideTimer);
            state.hideTimer = null;
        }
    }

    function scheduleShow() {
        cancelTimers();
        const delay = properties.popDelay || 0;
        if (delay > 0) {
            state.showTimer = setTimeout(showPopup, delay);
        } else {
            showPopup();
        }
    }

    function scheduleHide() {
        // Cancel any pending show
        if (state.showTimer) {
            clearTimeout(state.showTimer);
            state.showTimer = null;
        }
        const delay = properties.hoverDelay || 300;
        state.hideTimer = setTimeout(hidePopup, delay);
    }

    // Target hover handlers
    state.handlers.targetMouseEnter = function () {
        scheduleShow();
    };
    state.handlers.targetMouseLeave = function () {
        scheduleHide();
    };

    // Popup hover handlers — keep popup open while mouse is over it
    state.handlers.popupMouseEnter = function () {
        cancelTimers();
    };
    state.handlers.popupMouseLeave = function () {
        scheduleHide();
    };

    target.addEventListener("mouseenter", state.handlers.targetMouseEnter);
    target.addEventListener("mouseleave", state.handlers.targetMouseLeave);
    popup.addEventListener("mouseenter", state.handlers.popupMouseEnter);
    popup.addEventListener("mouseleave", state.handlers.popupMouseLeave);

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

    // Cancel any pending timers
    if (state.showTimer) clearTimeout(state.showTimer);
    if (state.hideTimer) clearTimeout(state.hideTimer);

    // Hide if visible
    if (state.isVisible) {
        state.popup.style.display = "none";
        if (state.properties.hoverCssClass) {
            const target = document.getElementById(state.targetId);
            if (target) {
                target.classList.remove(...state.properties.hoverCssClass.split(/\s+/).filter(Boolean));
            }
        }
    }

    const target = document.getElementById(state.targetId);
    if (target) {
        target.removeEventListener("mouseenter", state.handlers.targetMouseEnter);
        target.removeEventListener("mouseleave", state.handlers.targetMouseLeave);
    }

    state.popup.removeEventListener("mouseenter", state.handlers.popupMouseEnter);
    state.popup.removeEventListener("mouseleave", state.handlers.popupMouseLeave);

    behaviors.delete(behaviorId);
}
