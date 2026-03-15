// UpdatePanelAnimationExtender JS behavior module
// Provides visual feedback animations when content is updating.

const behaviors = new Map();

/**
 * Creates an update panel animation behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[UpdatePanelAnimationExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const {
        alwaysFinishOnUpdatingAnimation,
        onUpdatingCssClass,
        onUpdatedCssClass,
        fadeInDuration,
        fadeOutDuration
    } = properties;

    // Store original transition style so we can restore it
    const originalTransition = target.style.transition;
    const originalOpacity = target.style.opacity;

    // Track animation state
    let isUpdating = false;
    let updatingAnimationPromise = null;

    // Set up MutationObserver to detect content changes
    const observer = new MutationObserver((mutations) => {
        // Content has changed - trigger the "updated" animation
        if (isUpdating) {
            // Content changed while updating, transition to updated state
            finishUpdate();
        } else {
            // Content changed without explicit update start - show quick updated animation
            showUpdatedAnimation();
        }
    });

    observer.observe(target, {
        childList: true,
        subtree: true,
        characterData: true
    });

    /**
     * Starts the "updating" animation (fade-out and apply updating class)
     */
    function startUpdate() {
        isUpdating = true;

        // Apply fade-out transition
        target.style.transition = `opacity ${fadeOutDuration}s ease-out`;

        updatingAnimationPromise = new Promise((resolve) => {
            // Apply updating CSS class
            if (onUpdatingCssClass) {
                onUpdatingCssClass.split(' ').filter(c => c).forEach(c => target.classList.add(c));
            }

            // Remove updated CSS class if present
            if (onUpdatedCssClass) {
                onUpdatedCssClass.split(' ').filter(c => c).forEach(c => target.classList.remove(c));
            }

            // Fade out
            target.style.opacity = '0.5';

            // Resolve after fade-out completes
            setTimeout(resolve, fadeOutDuration * 1000);
        });

        return updatingAnimationPromise;
    }

    /**
     * Finishes the update and shows the "updated" animation (fade-in and apply updated class)
     */
    async function finishUpdate() {
        if (alwaysFinishOnUpdatingAnimation && updatingAnimationPromise) {
            await updatingAnimationPromise;
        }

        isUpdating = false;
        updatingAnimationPromise = null;

        showUpdatedAnimation();
    }

    /**
     * Shows the updated animation (fade-in effect)
     */
    function showUpdatedAnimation() {
        // Apply fade-in transition
        target.style.transition = `opacity ${fadeInDuration}s ease-in`;

        // Remove updating CSS class
        if (onUpdatingCssClass) {
            onUpdatingCssClass.split(' ').filter(c => c).forEach(c => target.classList.remove(c));
        }

        // Apply updated CSS class
        if (onUpdatedCssClass) {
            onUpdatedCssClass.split(' ').filter(c => c).forEach(c => target.classList.add(c));
        }

        // Fade in
        target.style.opacity = '1';

        // Remove updated class after animation completes (optional cleanup)
        setTimeout(() => {
            if (onUpdatedCssClass) {
                onUpdatedCssClass.split(' ').filter(c => c).forEach(c => target.classList.remove(c));
            }
        }, fadeInDuration * 1000);
    }

    // Expose methods on the target element for manual triggering
    target._updatePanelAnimation = {
        startUpdate,
        finishUpdate
    };

    const state = {
        targetId,
        target,
        alwaysFinishOnUpdatingAnimation,
        onUpdatingCssClass,
        onUpdatedCssClass,
        fadeInDuration,
        fadeOutDuration,
        originalTransition,
        originalOpacity,
        observer,
        isUpdating: () => isUpdating,
        startUpdate,
        finishUpdate
    };

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

    const {
        alwaysFinishOnUpdatingAnimation,
        onUpdatingCssClass,
        onUpdatedCssClass,
        fadeInDuration,
        fadeOutDuration
    } = properties;

    // Update stored properties
    state.alwaysFinishOnUpdatingAnimation = alwaysFinishOnUpdatingAnimation;
    state.onUpdatingCssClass = onUpdatingCssClass;
    state.onUpdatedCssClass = onUpdatedCssClass;
    state.fadeInDuration = fadeInDuration;
    state.fadeOutDuration = fadeOutDuration;
}

/**
 * Disposes the behavior and cleans up resources.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Disconnect the mutation observer
    if (state.observer) {
        state.observer.disconnect();
    }

    // Remove exposed methods from target element
    if (state.target && state.target._updatePanelAnimation) {
        delete state.target._updatePanelAnimation;
    }

    // Restore original styles
    if (state.target) {
        state.target.style.transition = state.originalTransition || "";
        state.target.style.opacity = state.originalOpacity || "";

        // Remove any applied CSS classes
        if (state.onUpdatingCssClass) {
            state.onUpdatingCssClass.split(' ').filter(c => c).forEach(c => {
                state.target.classList.remove(c);
            });
        }
        if (state.onUpdatedCssClass) {
            state.onUpdatedCssClass.split(' ').filter(c => c).forEach(c => {
                state.target.classList.remove(c);
            });
        }
    }

    behaviors.delete(behaviorId);
}

/**
 * Manually triggers the "updating" animation on a target.
 * Call this before starting an async operation.
 * @param {string} targetId - The ID of the target element
 */
export function triggerUpdating(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._updatePanelAnimation) {
        target._updatePanelAnimation.startUpdate();
    }
}

/**
 * Manually triggers the "updated" animation on a target.
 * Call this after an async operation completes.
 * @param {string} targetId - The ID of the target element
 */
export function triggerUpdated(targetId) {
    const target = document.getElementById(targetId);
    if (target && target._updatePanelAnimation) {
        target._updatePanelAnimation.finishUpdate();
    }
}
