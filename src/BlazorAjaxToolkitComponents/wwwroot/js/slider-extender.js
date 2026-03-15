// SliderExtender JS behavior module
// Attaches range slider behavior to a target input element using HTML5 input[type=range].
// Supports bound control sync, custom rail/handle CSS, and tooltip display.

const behaviors = new Map();

// SliderOrientation (matches C# enum)
const ORIENTATION_HORIZONTAL = 0;
const ORIENTATION_VERTICAL = 1;

/**
 * Creates a slider behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[SliderExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        handlers: {},
        sliderEl: null,
        tooltipEl: null
    };

    const isVertical = properties.orientation === ORIENTATION_VERTICAL;

    // Configure the target as a range input or wrap it
    let slider;
    if (target.tagName === "INPUT" && target.type === "range") {
        slider = target;
    } else if (target.tagName === "INPUT") {
        // Convert text input to range
        target.type = "range";
        slider = target;
    } else {
        // Create a range input inside the target element
        slider = document.createElement("input");
        slider.type = "range";
        target.appendChild(slider);
    }

    state.sliderEl = slider;

    // Apply properties
    slider.min = properties.minimum;
    slider.max = properties.maximum;
    slider.value = properties.value || properties.minimum;

    if (properties.steps > 0) {
        const stepSize = (properties.maximum - properties.minimum) / properties.steps;
        slider.step = properties.decimals > 0
            ? stepSize.toFixed(properties.decimals)
            : String(stepSize);
    } else if (properties.decimals > 0) {
        slider.step = Math.pow(10, -properties.decimals).toFixed(properties.decimals);
    }

    // Orientation
    if (isVertical) {
        slider.orient = "vertical"; // Firefox
        slider.style.writingMode = "vertical-lr"; // Chromium
        slider.style.direction = "rtl";
        if (properties.length > 0) {
            slider.style.height = `${properties.length}px`;
        }
    } else {
        if (properties.length > 0) {
            slider.style.width = `${properties.length}px`;
        }
    }

    // Apply CSS classes
    if (properties.railCssClass) {
        slider.classList.add(...properties.railCssClass.split(/\s+/).filter(Boolean));
    }
    if (properties.handleCssClass) {
        slider.dataset.handleCssClass = properties.handleCssClass;
    }

    // Handle image URL via CSS custom property
    if (properties.handleImageUrl) {
        slider.style.setProperty("--handle-image", `url('${properties.handleImageUrl}')`);
    }

    // Tooltip
    let tooltipEl = null;
    if (properties.tooltipText) {
        tooltipEl = document.createElement("span");
        tooltipEl.style.cssText = "position:absolute;pointer-events:none;opacity:0;transition:opacity 0.2s;z-index:9999;padding:2px 6px;background:#333;color:#fff;border-radius:3px;font-size:12px;white-space:nowrap;";
        document.body.appendChild(tooltipEl);
        state.tooltipEl = tooltipEl;

        function updateTooltipText() {
            const val = formatValue(slider.value);
            tooltipEl.textContent = properties.tooltipText.replace("{0}", val);
        }

        state.handlers.mouseEnter = function () {
            updateTooltipText();
            tooltipEl.style.opacity = "1";
        };
        state.handlers.mouseMove = function (e) {
            tooltipEl.style.left = `${e.pageX + 10}px`;
            tooltipEl.style.top = `${e.pageY - 25}px`;
        };
        state.handlers.mouseLeave = function () {
            tooltipEl.style.opacity = "0";
        };

        slider.addEventListener("mouseenter", state.handlers.mouseEnter);
        slider.addEventListener("mousemove", state.handlers.mouseMove);
        slider.addEventListener("mouseleave", state.handlers.mouseLeave);
    }

    function formatValue(val) {
        if (properties.decimals > 0) {
            return parseFloat(val).toFixed(properties.decimals);
        }
        return val;
    }

    // Bound control sync
    function syncBoundControl() {
        if (!properties.boundControlId) return;
        const bound = document.getElementById(properties.boundControlId);
        if (!bound) return;
        const val = formatValue(slider.value);
        if ("value" in bound) {
            bound.value = val;
        } else {
            bound.textContent = val;
        }
    }

    state.handlers.input = function () {
        syncBoundControl();
    };
    slider.addEventListener("input", state.handlers.input);

    // Initial sync
    syncBoundControl();

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

    if (state.sliderEl) {
        state.sliderEl.min = properties.minimum;
        state.sliderEl.max = properties.maximum;
        state.sliderEl.value = properties.value || properties.minimum;
    }
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const slider = state.sliderEl;
    if (slider) {
        if (state.handlers.input) {
            slider.removeEventListener("input", state.handlers.input);
        }
        if (state.handlers.mouseEnter) {
            slider.removeEventListener("mouseenter", state.handlers.mouseEnter);
        }
        if (state.handlers.mouseMove) {
            slider.removeEventListener("mousemove", state.handlers.mouseMove);
        }
        if (state.handlers.mouseLeave) {
            slider.removeEventListener("mouseleave", state.handlers.mouseLeave);
        }
    }

    if (state.tooltipEl) {
        state.tooltipEl.remove();
    }

    behaviors.delete(behaviorId);
}
