// RoundedCornersExtender JS behavior module
// Applies rounded corners to an element using CSS border-radius.

const behaviors = new Map();

// BoxCorners enum values (must match C# enum)
const BoxCorners = {
    None: 0,
    TopLeft: 1,
    TopRight: 2,
    BottomLeft: 4,
    BottomRight: 8,
    Top: 3,      // TopLeft | TopRight
    Bottom: 12,  // BottomLeft | BottomRight
    Left: 5,     // TopLeft | BottomLeft
    Right: 10,   // TopRight | BottomRight
    All: 15      // All corners
};

/**
 * Creates a rounded corners behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[RoundedCornersExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const { radius, corners, color } = properties;

    // Store original styles so we can restore them on dispose
    const originalBorderRadius = target.style.borderRadius;
    const originalBorderTopLeftRadius = target.style.borderTopLeftRadius;
    const originalBorderTopRightRadius = target.style.borderTopRightRadius;
    const originalBorderBottomLeftRadius = target.style.borderBottomLeftRadius;
    const originalBorderBottomRightRadius = target.style.borderBottomRightRadius;
    const originalBackgroundColor = target.style.backgroundColor;

    // Apply the rounded corners
    applyRoundedCorners(target, radius, corners);

    // Apply background color if specified
    if (color) {
        target.style.backgroundColor = color;
    }

    const state = {
        targetId,
        target,
        radius,
        corners,
        color,
        originalBorderRadius,
        originalBorderTopLeftRadius,
        originalBorderTopRightRadius,
        originalBorderBottomLeftRadius,
        originalBorderBottomRightRadius,
        originalBackgroundColor
    };

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Applies border-radius CSS to specific corners of the target element.
 * @param {HTMLElement} target - The element to apply corners to
 * @param {number} radius - Corner radius in pixels
 * @param {number} corners - BoxCorners flags indicating which corners to round
 */
function applyRoundedCorners(target, radius, corners) {
    const radiusValue = radius + "px";

    // If all corners, use shorthand
    if (corners === BoxCorners.All) {
        target.style.borderRadius = radiusValue;
        return;
    }

    // If no corners, clear any existing radius
    if (corners === BoxCorners.None) {
        target.style.borderRadius = "0";
        return;
    }

    // Apply individual corners based on flags
    target.style.borderTopLeftRadius = (corners & BoxCorners.TopLeft) ? radiusValue : "0";
    target.style.borderTopRightRadius = (corners & BoxCorners.TopRight) ? radiusValue : "0";
    target.style.borderBottomLeftRadius = (corners & BoxCorners.BottomLeft) ? radiusValue : "0";
    target.style.borderBottomRightRadius = (corners & BoxCorners.BottomRight) ? radiusValue : "0";
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const { radius, corners, color } = properties;

    // Update corners if radius or corners changed
    if (radius !== state.radius || corners !== state.corners) {
        applyRoundedCorners(state.target, radius, corners);
        state.radius = radius;
        state.corners = corners;
    }

    // Update background color if changed
    if (color !== state.color) {
        if (color) {
            state.target.style.backgroundColor = color;
        } else {
            state.target.style.backgroundColor = state.originalBackgroundColor || "";
        }
        state.color = color;
    }
}

/**
 * Disposes the behavior and removes the rounded corners effect.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Restore original styles
    if (state.target) {
        state.target.style.borderRadius = state.originalBorderRadius || "";
        state.target.style.borderTopLeftRadius = state.originalBorderTopLeftRadius || "";
        state.target.style.borderTopRightRadius = state.originalBorderTopRightRadius || "";
        state.target.style.borderBottomLeftRadius = state.originalBorderBottomLeftRadius || "";
        state.target.style.borderBottomRightRadius = state.originalBorderBottomRightRadius || "";
        state.target.style.backgroundColor = state.originalBackgroundColor || "";
    }

    behaviors.delete(behaviorId);
}
