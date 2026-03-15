// ToggleButtonExtender JS behavior module
// Replaces a target checkbox with a clickable image that toggles
// between checked and unchecked states, with hover and disabled image support.

const behaviors = new Map();

/**
 * Creates a toggle button behavior and attaches it to the target checkbox.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[ToggleButtonExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        handlers: {},
        imageEl: null,
        checkbox: null
    };

    // Find the checkbox — target could be the checkbox itself or a container
    let checkbox;
    if (target.tagName === "INPUT" && target.type === "checkbox") {
        checkbox = target;
    } else {
        checkbox = target.querySelector('input[type="checkbox"]');
    }

    if (!checkbox) {
        console.warn(`[ToggleButtonExtender] No checkbox found in target '${targetId}'.`);
        return {};
    }

    state.checkbox = checkbox;

    // Hide the checkbox
    checkbox.style.display = "none";

    // Create the toggle image
    const img = document.createElement("img");
    img.style.cursor = "pointer";
    if (properties.imageWidth > 0) img.width = properties.imageWidth;
    if (properties.imageHeight > 0) img.height = properties.imageHeight;

    state.imageEl = img;

    function getImageUrl() {
        const isChecked = checkbox.checked;
        const isDisabled = checkbox.disabled;

        if (isDisabled) {
            return isChecked
                ? (properties.disabledCheckedImageUrl || properties.checkedImageUrl)
                : (properties.disabledUncheckedImageUrl || properties.uncheckedImageUrl);
        }

        return isChecked ? properties.checkedImageUrl : properties.uncheckedImageUrl;
    }

    function getAltText() {
        return checkbox.checked
            ? (properties.checkedImageAlternateText || "Checked")
            : (properties.uncheckedImageAlternateText || "Unchecked");
    }

    function updateImage() {
        img.src = getImageUrl();
        img.alt = getAltText();
    }

    // Initial render
    updateImage();

    // Insert image after checkbox
    checkbox.parentNode.insertBefore(img, checkbox.nextSibling);

    // Click handler — toggle the checkbox
    state.handlers.click = function (e) {
        if (checkbox.disabled) return;
        e.preventDefault();
        checkbox.checked = !checkbox.checked;
        // Dispatch change event so Blazor picks up the change
        checkbox.dispatchEvent(new Event("change", { bubbles: true }));
        updateImage();
    };
    img.addEventListener("click", state.handlers.click);

    // Hover handlers for image swapping
    state.handlers.mouseEnter = function () {
        if (checkbox.disabled) return;
        const hoverUrl = checkbox.checked
            ? properties.checkedImageOverUrl
            : properties.uncheckedImageOverUrl;
        if (hoverUrl) {
            img.src = hoverUrl;
        }
    };
    state.handlers.mouseLeave = function () {
        updateImage();
    };
    img.addEventListener("mouseenter", state.handlers.mouseEnter);
    img.addEventListener("mouseleave", state.handlers.mouseLeave);

    // Keyboard accessibility — space/enter toggles
    img.tabIndex = 0;
    img.role = "checkbox";
    img.setAttribute("aria-checked", String(checkbox.checked));

    state.handlers.keydown = function (e) {
        if (checkbox.disabled) return;
        if (e.key === " " || e.key === "Enter") {
            e.preventDefault();
            checkbox.checked = !checkbox.checked;
            checkbox.dispatchEvent(new Event("change", { bubbles: true }));
            img.setAttribute("aria-checked", String(checkbox.checked));
            updateImage();
        }
    };
    img.addEventListener("keydown", state.handlers.keydown);

    // Watch for external checkbox changes
    state.handlers.change = function () {
        img.setAttribute("aria-checked", String(checkbox.checked));
        updateImage();
    };
    checkbox.addEventListener("change", state.handlers.change);

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

    // Re-render current image state
    if (state.imageEl && state.checkbox) {
        const isChecked = state.checkbox.checked;
        const isDisabled = state.checkbox.disabled;
        if (isDisabled) {
            state.imageEl.src = isChecked
                ? (properties.disabledCheckedImageUrl || properties.checkedImageUrl)
                : (properties.disabledUncheckedImageUrl || properties.uncheckedImageUrl);
        } else {
            state.imageEl.src = isChecked ? properties.checkedImageUrl : properties.uncheckedImageUrl;
        }
        if (properties.imageWidth > 0) state.imageEl.width = properties.imageWidth;
        if (properties.imageHeight > 0) state.imageEl.height = properties.imageHeight;
    }
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    if (state.imageEl) {
        state.imageEl.removeEventListener("click", state.handlers.click);
        state.imageEl.removeEventListener("mouseenter", state.handlers.mouseEnter);
        state.imageEl.removeEventListener("mouseleave", state.handlers.mouseLeave);
        state.imageEl.removeEventListener("keydown", state.handlers.keydown);
        state.imageEl.remove();
    }

    if (state.checkbox) {
        state.checkbox.removeEventListener("change", state.handlers.change);
        state.checkbox.style.display = "";
    }

    behaviors.delete(behaviorId);
}
