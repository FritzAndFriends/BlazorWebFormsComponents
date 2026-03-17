// PopupControlExtender JS behavior module
// Displays a popup panel attached to a target control on click.
// Lighter than ModalPopupExtender — no overlay, no focus trap.
// Supports positional placement, commit property/script, and outside-click dismiss.

const behaviors = new Map();

// PopupPosition (matches C# enum)
const POSITION_LEFT = 0;
const POSITION_RIGHT = 1;
const POSITION_TOP = 2;
const POSITION_BOTTOM = 3;
const POSITION_CENTER = 4;

/**
 * Creates a popup control behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const trigger = document.getElementById(targetId);

    if (!trigger) {
        console.warn(`[PopupControlExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const popup = document.getElementById(properties.popupControlId);
    if (!popup) {
        console.warn(`[PopupControlExtender] Popup element '${properties.popupControlId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        popup,
        isOpen: false,
        handlers: {}
    };

    // Hide popup initially
    popup.style.display = "none";

    function positionPopup() {
        const triggerRect = trigger.getBoundingClientRect();
        const popupRect = popup.getBoundingClientRect();
        const scrollX = window.scrollX || window.pageXOffset;
        const scrollY = window.scrollY || window.pageYOffset;
        const offsetX = properties.offsetX || 0;
        const offsetY = properties.offsetY || 0;

        let left, top;

        switch (properties.position) {
            case POSITION_LEFT:
                left = triggerRect.left - popupRect.width + scrollX + offsetX;
                top = triggerRect.top + scrollY + offsetY;
                break;
            case POSITION_RIGHT:
                left = triggerRect.right + scrollX + offsetX;
                top = triggerRect.top + scrollY + offsetY;
                break;
            case POSITION_TOP:
                left = triggerRect.left + scrollX + offsetX;
                top = triggerRect.top - popupRect.height + scrollY + offsetY;
                break;
            case POSITION_CENTER:
                left = triggerRect.left + (triggerRect.width - popupRect.width) / 2 + scrollX + offsetX;
                top = triggerRect.top + (triggerRect.height - popupRect.height) / 2 + scrollY + offsetY;
                break;
            case POSITION_BOTTOM:
            default:
                left = triggerRect.left + scrollX + offsetX;
                top = triggerRect.bottom + scrollY + offsetY;
                break;
        }

        popup.style.left = `${Math.max(0, left)}px`;
        popup.style.top = `${Math.max(0, top)}px`;
    }

    function show() {
        if (state.isOpen) return;
        state.isOpen = true;

        popup.style.display = "";
        popup.style.position = "absolute";
        popup.style.zIndex = "9999";
        positionPopup();

        // Add outside-click dismiss with a short delay to avoid immediate dismissal
        requestAnimationFrame(() => {
            document.addEventListener("mousedown", state.handlers.outsideClick);
        });
    }

    function hide() {
        if (!state.isOpen) return;
        state.isOpen = false;

        popup.style.display = "none";
        document.removeEventListener("mousedown", state.handlers.outsideClick);
    }

    function commit(value) {
        // Set commit property on target
        if (properties.commitProperty && value !== undefined) {
            trigger[properties.commitProperty] = value;
        }

        // Run commit script
        if (properties.commitScript) {
            try {
                new Function(properties.commitScript)();
            } catch (e) {
                console.error("[PopupControlExtender] Commit script error:", e);
            }
        }

        hide();
    }

    // Outside click handler
    state.handlers.outsideClick = function (e) {
        if (!popup.contains(e.target) && !trigger.contains(e.target)) {
            hide();
        }
    };

    // Trigger click toggles the popup
    state.handlers.triggerClick = function (e) {
        e.preventDefault();
        if (state.isOpen) {
            hide();
        } else {
            show();
        }
    };
    trigger.addEventListener("click", state.handlers.triggerClick);

    // Escape key closes popup
    state.handlers.keydown = function (e) {
        if (e.key === "Escape" && state.isOpen) {
            hide();
        }
    };
    document.addEventListener("keydown", state.handlers.keydown);

    state.show = show;
    state.hide = hide;
    state.commit = commit;
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
        state.hide();
    }

    // Remove trigger listener
    const trigger = document.getElementById(state.targetId);
    if (trigger && state.handlers.triggerClick) {
        trigger.removeEventListener("click", state.handlers.triggerClick);
    }

    document.removeEventListener("keydown", state.handlers.keydown);
    document.removeEventListener("mousedown", state.handlers.outsideClick);

    behaviors.delete(behaviorId);
}
