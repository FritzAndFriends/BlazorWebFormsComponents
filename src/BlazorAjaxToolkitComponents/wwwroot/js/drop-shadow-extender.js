// DropShadowExtender JS behavior module
// Adds a drop shadow effect to an element, giving it a raised/floating appearance.

const behaviors = new Map();

/**
 * Creates a drop shadow behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[DropShadowExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const { opacity, width, rounded, radius, trackPosition } = properties;

    // Store original styles so we can restore them on dispose
    const originalBoxShadow = target.style.boxShadow;
    const originalBorderRadius = target.style.borderRadius;

    // Apply the drop shadow effect
    applyDropShadow(target, opacity, width);

    // Apply rounded corners if enabled
    if (rounded && radius > 0) {
        target.style.borderRadius = radius + "px";
    }

    // Set up position tracking if enabled
    let positionObserver = null;
    if (trackPosition) {
        positionObserver = createPositionObserver(target, opacity, width);
    }

    const state = {
        targetId,
        target,
        opacity,
        width,
        rounded,
        radius,
        trackPosition,
        originalBoxShadow,
        originalBorderRadius,
        positionObserver
    };

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Applies the drop shadow CSS to the target element.
 * @param {HTMLElement} target - The element to apply shadow to
 * @param {number} opacity - Shadow opacity (0.0 to 1.0)
 * @param {number} width - Shadow width in pixels
 */
function applyDropShadow(target, opacity, width) {
    // Clamp opacity between 0 and 1
    const clampedOpacity = Math.max(0, Math.min(1, opacity));
    
    // Create the box-shadow CSS value
    // Using offset-x, offset-y, blur-radius, and rgba color
    const shadowColor = `rgba(0, 0, 0, ${clampedOpacity})`;
    target.style.boxShadow = `${width}px ${width}px ${width}px ${shadowColor}`;
}

/**
 * Creates a MutationObserver to track position/style changes and reapply shadow.
 * This ensures the shadow stays consistent if the element moves or changes.
 * @param {HTMLElement} target - The element to observe
 * @param {number} opacity - Shadow opacity
 * @param {number} width - Shadow width
 * @returns {MutationObserver} The observer instance
 */
function createPositionObserver(target, opacity, width) {
    const observer = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === "attributes" && mutation.attributeName === "style") {
                // Re-ensure shadow is applied (in case something else modified it)
                const currentShadow = target.style.boxShadow;
                if (!currentShadow || currentShadow === "none") {
                    applyDropShadow(target, opacity, width);
                }
            }
        }
    });

    observer.observe(target, {
        attributes: true,
        attributeFilter: ["style", "class"]
    });

    return observer;
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const { opacity, width, rounded, radius, trackPosition } = properties;

    // Update shadow if opacity or width changed
    if (opacity !== state.opacity || width !== state.width) {
        applyDropShadow(state.target, opacity, width);
        state.opacity = opacity;
        state.width = width;
    }

    // Update rounded corners
    if (rounded !== state.rounded || radius !== state.radius) {
        if (rounded && radius > 0) {
            state.target.style.borderRadius = radius + "px";
        } else {
            state.target.style.borderRadius = state.originalBorderRadius || "";
        }
        state.rounded = rounded;
        state.radius = radius;
    }

    // Update position tracking
    if (trackPosition !== state.trackPosition) {
        if (trackPosition && !state.positionObserver) {
            state.positionObserver = createPositionObserver(state.target, opacity, width);
        } else if (!trackPosition && state.positionObserver) {
            state.positionObserver.disconnect();
            state.positionObserver = null;
        }
        state.trackPosition = trackPosition;
    }
}

/**
 * Disposes the behavior and removes the drop shadow effect.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Disconnect position observer if active
    if (state.positionObserver) {
        state.positionObserver.disconnect();
    }

    // Restore original styles
    if (state.target) {
        state.target.style.boxShadow = state.originalBoxShadow || "";
        state.target.style.borderRadius = state.originalBorderRadius || "";
    }

    behaviors.delete(behaviorId);
}
