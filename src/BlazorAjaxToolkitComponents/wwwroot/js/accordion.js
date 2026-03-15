// Accordion JS behavior module
// Provides smooth expand/collapse animations for Accordion panes.

const accordions = new Map();

/**
 * Initializes an Accordion instance with animation support.
 * @param {string} containerId - The DOM ID of the accordion container element.
 * @param {{ selectedIndex: number, transitionDuration: number, fadeTransitions: boolean, autoSize: number }} config
 */
export function initAccordion(containerId, config) {
    const container = document.getElementById(containerId);
    if (!container) return;

    const state = {
        containerId,
        config: { ...config }
    };

    // Set up content panes for animation
    const contentPanes = container.querySelectorAll("[data-pane-index][role='tabpanel']");
    contentPanes.forEach((pane, index) => {
        pane.style.overflow = "hidden";
        pane.style.transition = `height ${config.transitionDuration}ms ease` +
            (config.fadeTransitions ? `, opacity ${config.transitionDuration}ms ease` : "");

        if (index === config.selectedIndex) {
            pane.style.display = "";
            pane.style.height = "auto";
            if (config.fadeTransitions) pane.style.opacity = "1";
        } else {
            pane.style.display = "none";
            pane.style.height = "0";
            if (config.fadeTransitions) pane.style.opacity = "0";
        }
    });

    accordions.set(containerId, state);
}

/**
 * Animates the transition from one pane to another.
 * @param {string} containerId
 * @param {number} oldIndex - Index of the pane to collapse (-1 if none).
 * @param {number} newIndex - Index of the pane to expand (-1 to collapse all).
 * @param {number} duration - Transition duration in ms.
 * @param {boolean} fade - Whether to include opacity transition.
 */
export function selectPane(containerId, oldIndex, newIndex, duration, fade) {
    const container = document.getElementById(containerId);
    if (!container) return;

    const contentPanes = container.querySelectorAll("[data-pane-index][role='tabpanel']");

    // Collapse old pane
    if (oldIndex >= 0 && oldIndex < contentPanes.length) {
        const oldPane = contentPanes[oldIndex];
        const currentHeight = oldPane.scrollHeight;
        oldPane.style.transition = "none";
        oldPane.style.height = currentHeight + "px";
        oldPane.style.display = "";

        // Force reflow
        oldPane.offsetHeight;

        oldPane.style.transition = `height ${duration}ms ease` +
            (fade ? `, opacity ${duration}ms ease` : "");
        oldPane.style.height = "0";
        if (fade) oldPane.style.opacity = "0";

        // Hide after transition completes
        const hideOld = () => {
            if (oldPane.style.height === "0px" || oldPane.style.height === "0") {
                oldPane.style.display = "none";
            }
            oldPane.removeEventListener("transitionend", hideOld);
        };
        oldPane.addEventListener("transitionend", hideOld);
    }

    // Expand new pane
    if (newIndex >= 0 && newIndex < contentPanes.length) {
        const newPane = contentPanes[newIndex];
        newPane.style.transition = "none";
        newPane.style.display = "";
        newPane.style.height = "0";
        if (fade) newPane.style.opacity = "0";

        // Force reflow
        newPane.offsetHeight;

        // Measure natural height
        newPane.style.height = "auto";
        const targetHeight = newPane.scrollHeight;
        newPane.style.height = "0";

        // Force reflow again
        newPane.offsetHeight;

        newPane.style.transition = `height ${duration}ms ease` +
            (fade ? `, opacity ${duration}ms ease` : "");
        newPane.style.height = targetHeight + "px";
        if (fade) newPane.style.opacity = "1";

        // Set to auto after transition for responsive sizing
        const expandDone = (e) => {
            if (e.propertyName === "height") {
                newPane.style.height = "auto";
            }
            newPane.removeEventListener("transitionend", expandDone);
        };
        newPane.addEventListener("transitionend", expandDone);
    }
}

/**
 * Disposes the accordion behavior and cleans up state.
 * @param {string} containerId
 */
export function disposeAccordion(containerId) {
    accordions.delete(containerId);
}
