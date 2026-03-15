// CollapsiblePanelExtender JS behavior module
// Adds collapse/expand behavior to a target panel using CSS transitions.

const behaviors = new Map();

// ExpandDirection (matches C# enum)
const DIRECTION_VERTICAL = 0;
const DIRECTION_HORIZONTAL = 1;

/**
 * Creates a collapsible panel behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[CollapsiblePanelExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const isVertical = (properties.expandDirection || DIRECTION_VERTICAL) === DIRECTION_VERTICAL;
    const sizeProp = isVertical ? "height" : "width";
    const overflowProp = isVertical ? "overflowY" : "overflowX";

    const state = {
        targetId,
        properties: { ...properties },
        isCollapsed: !!properties.collapsed,
        isVertical,
        sizeProp,
        handlers: {}
    };

    // Set up CSS transition on the target
    target.style.transition = `${sizeProp} 0.3s ease`;
    target.style.overflow = "hidden";

    if (properties.scrollContents) {
        target.style[overflowProp] = "auto";
    }

    function getExpandedSize() {
        if (properties.expandedSize > 0) {
            return `${properties.expandedSize}px`;
        }
        // Measure natural size
        const prev = target.style[sizeProp];
        target.style[sizeProp] = "auto";
        const measured = isVertical ? target.scrollHeight : target.scrollWidth;
        target.style[sizeProp] = prev;
        return `${measured}px`;
    }

    function getCollapsedSize() {
        return `${properties.collapsedSize || 0}px`;
    }

    function updateLabel() {
        if (!properties.textLabelId) return;
        const label = document.getElementById(properties.textLabelId);
        if (!label) return;
        label.textContent = state.isCollapsed ? properties.collapsedText : properties.expandedText;
    }

    function collapse() {
        if (state.isCollapsed) return;
        // Set explicit size first so transition animates from current to collapsed
        const currentSize = isVertical ? target.scrollHeight : target.scrollWidth;
        target.style[sizeProp] = `${currentSize}px`;
        // Force reflow
        target.offsetHeight; // eslint-disable-line no-unused-expressions
        target.style[sizeProp] = getCollapsedSize();
        state.isCollapsed = true;
        updateLabel();
    }

    function expand() {
        if (!state.isCollapsed) return;
        const expandedSize = getExpandedSize();
        target.style[sizeProp] = expandedSize;
        state.isCollapsed = false;
        updateLabel();

        // After transition, set to auto if no fixed expanded size
        if (!properties.expandedSize) {
            function onTransitionEnd(e) {
                if (e.propertyName === sizeProp && !state.isCollapsed) {
                    target.style[sizeProp] = "auto";
                }
                target.removeEventListener("transitionend", onTransitionEnd);
            }
            target.addEventListener("transitionend", onTransitionEnd);
        }
    }

    function toggle() {
        if (state.isCollapsed) {
            expand();
        } else {
            collapse();
        }
    }

    // Apply initial state without transition
    target.style.transition = "none";
    if (state.isCollapsed) {
        target.style[sizeProp] = getCollapsedSize();
    } else if (properties.expandedSize > 0) {
        target.style[sizeProp] = `${properties.expandedSize}px`;
    }
    updateLabel();
    // Re-enable transitions after initial layout
    requestAnimationFrame(() => {
        target.style.transition = `${sizeProp} 0.3s ease`;
    });

    // Bind collapse/expand controls
    const collapseControlId = properties.collapseControlId;
    const expandControlId = properties.expandControlId;
    const isSameControl = collapseControlId && collapseControlId === expandControlId;

    if (isSameControl) {
        const toggleBtn = document.getElementById(collapseControlId);
        if (toggleBtn) {
            state.handlers.toggleClick = (e) => { e.preventDefault(); toggle(); };
            toggleBtn.addEventListener("click", state.handlers.toggleClick);
            state.handlers.toggleBtn = toggleBtn;
        }
    } else {
        if (collapseControlId) {
            const collapseBtn = document.getElementById(collapseControlId);
            if (collapseBtn) {
                state.handlers.collapseClick = (e) => { e.preventDefault(); collapse(); };
                collapseBtn.addEventListener("click", state.handlers.collapseClick);
                state.handlers.collapseBtn = collapseBtn;
            }
        }
        if (expandControlId) {
            const expandBtn = document.getElementById(expandControlId);
            if (expandBtn) {
                state.handlers.expandClick = (e) => { e.preventDefault(); expand(); };
                expandBtn.addEventListener("click", state.handlers.expandClick);
                state.handlers.expandBtn = expandBtn;
            }
        }
    }

    // Auto-expand on mouse enter
    if (properties.autoExpand) {
        state.handlers.mouseEnter = () => expand();
        target.addEventListener("mouseenter", state.handlers.mouseEnter);
    }

    // Auto-collapse on mouse leave
    if (properties.autoCollapse) {
        state.handlers.mouseLeave = () => collapse();
        target.addEventListener("mouseleave", state.handlers.mouseLeave);
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
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);

    if (state.handlers.toggleBtn) {
        state.handlers.toggleBtn.removeEventListener("click", state.handlers.toggleClick);
    }
    if (state.handlers.collapseBtn) {
        state.handlers.collapseBtn.removeEventListener("click", state.handlers.collapseClick);
    }
    if (state.handlers.expandBtn) {
        state.handlers.expandBtn.removeEventListener("click", state.handlers.expandClick);
    }
    if (target && state.handlers.mouseEnter) {
        target.removeEventListener("mouseenter", state.handlers.mouseEnter);
    }
    if (target && state.handlers.mouseLeave) {
        target.removeEventListener("mouseleave", state.handlers.mouseLeave);
    }

    behaviors.delete(behaviorId);
}
