// NumericUpDownExtender JS behavior module
// Adds up/down spinner buttons and keyboard increment/decrement to a target input.

const behaviors = new Map();

/**
 * Creates a numeric up/down behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[NumericUpDownExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const refValues = (properties.refValues || "").split(";").filter(v => v.length > 0);
    const useRefValues = refValues.length > 0;

    const state = {
        targetId,
        properties: { ...properties },
        refValues,
        useRefValues,
        wrapper: null,
        upBtn: null,
        downBtn: null,
        handlers: {}
    };

    // Wrap the target in a container with up/down buttons
    const wrapper = document.createElement("span");
    wrapper.style.display = "inline-flex";
    wrapper.style.alignItems = "center";
    if (properties.width) {
        wrapper.style.width = `${properties.width}px`;
    }
    target.parentNode.insertBefore(wrapper, target);
    wrapper.appendChild(target);
    state.wrapper = wrapper;

    const btnStyle = "border:1px solid #ccc;background:#f0f0f0;cursor:pointer;padding:2px 6px;font-size:12px;line-height:1;user-select:none;";

    const btnContainer = document.createElement("span");
    btnContainer.style.display = "inline-flex";
    btnContainer.style.flexDirection = "column";
    btnContainer.style.marginLeft = "2px";

    const upBtn = document.createElement("button");
    upBtn.type = "button";
    upBtn.setAttribute("style", btnStyle);
    upBtn.textContent = "\u25B2";
    upBtn.setAttribute("aria-label", "Increment");

    const downBtn = document.createElement("button");
    downBtn.type = "button";
    downBtn.setAttribute("style", btnStyle);
    downBtn.textContent = "\u25BC";
    downBtn.setAttribute("aria-label", "Decrement");

    btnContainer.appendChild(upBtn);
    btnContainer.appendChild(downBtn);
    wrapper.appendChild(btnContainer);
    state.upBtn = upBtn;
    state.downBtn = downBtn;

    function getCurrentRefIndex() {
        const val = target.value.trim();
        const idx = refValues.indexOf(val);
        return idx >= 0 ? idx : 0;
    }

    function clamp(val) {
        const min = properties.minimum != null ? properties.minimum : -Number.MAX_VALUE;
        const max = properties.maximum != null ? properties.maximum : Number.MAX_VALUE;
        return Math.min(Math.max(val, min), max);
    }

    function stepUp() {
        if (useRefValues) {
            const idx = getCurrentRefIndex();
            const nextIdx = (idx + 1) % refValues.length;
            target.value = refValues[nextIdx];
        } else {
            const current = parseFloat(target.value) || 0;
            const step = properties.step || 1;
            target.value = clamp(current + step);
        }
        target.dispatchEvent(new Event("input", { bubbles: true }));
        target.dispatchEvent(new Event("change", { bubbles: true }));
    }

    function stepDown() {
        if (useRefValues) {
            const idx = getCurrentRefIndex();
            const nextIdx = (idx - 1 + refValues.length) % refValues.length;
            target.value = refValues[nextIdx];
        } else {
            const current = parseFloat(target.value) || 0;
            const step = properties.step || 1;
            target.value = clamp(current - step);
        }
        target.dispatchEvent(new Event("input", { bubbles: true }));
        target.dispatchEvent(new Event("change", { bubbles: true }));
    }

    state.handlers.upClick = () => stepUp();
    state.handlers.downClick = () => stepDown();
    upBtn.addEventListener("click", state.handlers.upClick);
    downBtn.addEventListener("click", state.handlers.downClick);

    // Keyboard support: ArrowUp/ArrowDown
    state.handlers.keydown = function (e) {
        if (e.key === "ArrowUp") {
            e.preventDefault();
            stepUp();
        } else if (e.key === "ArrowDown") {
            e.preventDefault();
            stepDown();
        }
    };
    target.addEventListener("keydown", state.handlers.keydown);

    // Initialize with first refValue if no value set
    if (useRefValues && !target.value) {
        target.value = refValues[0];
    }

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
    const newRefValues = (properties.refValues || "").split(";").filter(v => v.length > 0);
    state.refValues = newRefValues;
    state.useRefValues = newRefValues.length > 0;
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
        target.removeEventListener("keydown", state.handlers.keydown);
    }
    if (state.upBtn) {
        state.upBtn.removeEventListener("click", state.handlers.upClick);
    }
    if (state.downBtn) {
        state.downBtn.removeEventListener("click", state.handlers.downClick);
    }

    // Unwrap — move target back to original position
    if (state.wrapper && state.wrapper.parentNode) {
        const parent = state.wrapper.parentNode;
        if (target) {
            parent.insertBefore(target, state.wrapper);
        }
        state.wrapper.remove();
    }

    behaviors.delete(behaviorId);
}
