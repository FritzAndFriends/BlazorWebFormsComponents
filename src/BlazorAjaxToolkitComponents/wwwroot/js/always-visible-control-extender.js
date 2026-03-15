// AlwaysVisibleControlExtender JS behavior module
// Keeps a control visible in a fixed position on screen even when scrolling.

const behaviors = new Map();

/**
 * Creates the always-visible behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[AlwaysVisibleControlExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        target,
        properties: { ...properties },
        originalStyles: {
            position: target.style.position,
            left: target.style.left,
            right: target.style.right,
            top: target.style.top,
            bottom: target.style.bottom,
            transform: target.style.transform,
            transition: target.style.transition,
            zIndex: target.style.zIndex
        }
    };

    applyFixedPosition(state);
    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Applies the fixed positioning styles to the target element.
 * @param {object} state - The behavior state object.
 */
function applyFixedPosition(state) {
    const { target, properties } = state;
    const {
        horizontalOffset = 0,
        verticalOffset = 0,
        horizontalSide = "left",
        verticalSide = "top",
        scrollEffectDuration = 0.1,
        useAnimation = true
    } = properties;

    // Set fixed positioning
    target.style.position = "fixed";
    target.style.zIndex = "9999";

    // Apply transition if animation is enabled
    if (useAnimation) {
        target.style.transition = `left ${scrollEffectDuration}s ease-out, right ${scrollEffectDuration}s ease-out, top ${scrollEffectDuration}s ease-out, bottom ${scrollEffectDuration}s ease-out, transform ${scrollEffectDuration}s ease-out`;
    } else {
        target.style.transition = "none";
    }

    // Reset positioning properties
    target.style.left = "";
    target.style.right = "";
    target.style.top = "";
    target.style.bottom = "";
    target.style.transform = "";

    // Apply horizontal positioning
    switch (horizontalSide) {
        case "left":
            target.style.left = `${horizontalOffset}px`;
            break;
        case "right":
            target.style.right = `${horizontalOffset}px`;
            break;
        case "center":
            target.style.left = "50%";
            target.style.transform = "translateX(-50%)";
            if (horizontalOffset !== 0) {
                target.style.marginLeft = `${horizontalOffset}px`;
            }
            break;
    }

    // Apply vertical positioning
    switch (verticalSide) {
        case "top":
            target.style.top = `${verticalOffset}px`;
            break;
        case "bottom":
            target.style.bottom = `${verticalOffset}px`;
            break;
        case "middle":
            // Handle center+middle combination
            if (horizontalSide === "center") {
                target.style.transform = "translate(-50%, -50%)";
            } else {
                target.style.top = "50%";
                target.style.transform = "translateY(-50%)";
            }
            if (verticalOffset !== 0) {
                target.style.marginTop = `${verticalOffset}px`;
            }
            // For middle, also set top to 50% if not already handled
            if (horizontalSide !== "center") {
                target.style.top = "50%";
            } else {
                target.style.top = "50%";
            }
            break;
    }
}

/**
 * Updates behavior properties and re-applies positioning.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Update properties
    state.properties = { ...properties };

    // Re-apply positioning
    applyFixedPosition(state);
}

/**
 * Disposes the behavior and restores original styles.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const { target, originalStyles } = state;

    // Restore original styles
    if (target) {
        target.style.position = originalStyles.position;
        target.style.left = originalStyles.left;
        target.style.right = originalStyles.right;
        target.style.top = originalStyles.top;
        target.style.bottom = originalStyles.bottom;
        target.style.transform = originalStyles.transform;
        target.style.transition = originalStyles.transition;
        target.style.zIndex = originalStyles.zIndex;
        target.style.marginLeft = "";
        target.style.marginTop = "";
    }

    behaviors.delete(behaviorId);
}
