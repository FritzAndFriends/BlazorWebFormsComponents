// ConfirmButtonExtender JS behavior module
// Attaches a confirmation dialog to a target element's click event.

const behaviors = new Map();

/**
 * Creates a confirm behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: { confirmText: string, confirmOnFormSubmit: boolean, displayModalPopupId: string } }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[ConfirmButtonExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        confirmText: properties.confirmText || "Are you sure?",
        confirmOnFormSubmit: properties.confirmOnFormSubmit || false,
        displayModalPopupId: properties.displayModalPopupId || "",
        handler: null,
        formHandler: null
    };

    function clickHandler(e) {
        if (!window.confirm(state.confirmText)) {
            e.preventDefault();
            e.stopPropagation();
        }
    }

    function formSubmitHandler(e) {
        if (!window.confirm(state.confirmText)) {
            e.preventDefault();
            e.stopPropagation();
        }
    }

    if (state.confirmOnFormSubmit) {
        const form = target.closest("form");
        if (form) {
            state.formHandler = formSubmitHandler;
            form.addEventListener("submit", formSubmitHandler);
        }
    } else {
        state.handler = clickHandler;
        target.addEventListener("click", clickHandler);
    }

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {{ confirmText: string, confirmOnFormSubmit: boolean, displayModalPopupId: string }} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    state.confirmText = properties.confirmText || "Are you sure?";
    state.confirmOnFormSubmit = properties.confirmOnFormSubmit || false;
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);
    if (target && state.handler) {
        target.removeEventListener("click", state.handler);
    }

    if (state.formHandler) {
        const form = target?.closest("form");
        if (form) {
            form.removeEventListener("submit", state.formHandler);
        }
    }

    behaviors.delete(behaviorId);
}
