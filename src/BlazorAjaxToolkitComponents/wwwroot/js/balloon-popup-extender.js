// BalloonPopupExtender JS behavior module
// Displays a balloon/tooltip-style popup with a pointer arrow when the user
// hovers over, clicks, or focuses on a target element.

const behaviors = new Map();

// BalloonPosition (matches C# enum)
const POSITION_TOP_LEFT = 0;
const POSITION_TOP_RIGHT = 1;
const POSITION_BOTTOM_LEFT = 2;
const POSITION_BOTTOM_RIGHT = 3;
const POSITION_AUTO = 4;

// BalloonStyle (matches C# enum)
const STYLE_RECTANGLE = 0;
const STYLE_CLOUD = 1;
const STYLE_CUSTOM = 2;

// BalloonSize (matches C# enum)
const SIZE_SMALL = 0;
const SIZE_MEDIUM = 1;
const SIZE_LARGE = 2;

// ScrollBars (matches C# enum)
const SCROLLBARS_NONE = 0;
const SCROLLBARS_HORIZONTAL = 1;
const SCROLLBARS_VERTICAL = 2;
const SCROLLBARS_BOTH = 3;
const SCROLLBARS_AUTO = 4;

// Size presets (width x height in pixels)
const SIZE_PRESETS = {
    [SIZE_SMALL]: { width: 150, maxHeight: 100 },
    [SIZE_MEDIUM]: { width: 250, maxHeight: 200 },
    [SIZE_LARGE]: { width: 350, maxHeight: 300 }
};

/**
 * Creates a balloon popup behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[BalloonPopupExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const contentElement = document.getElementById(properties.balloonPopupControlId);
    if (!contentElement) {
        console.warn(`[BalloonPopupExtender] Balloon content element '${properties.balloonPopupControlId}' not found.`);
        return {};
    }

    // Create balloon container with arrow
    const balloon = createBalloonContainer(properties);
    document.body.appendChild(balloon);

    // Move content into balloon
    const contentWrapper = balloon.querySelector('.balloon-content');
    const originalDisplay = contentElement.style.display;
    contentElement.style.display = '';
    contentWrapper.appendChild(contentElement);

    const state = {
        targetId,
        properties: { ...properties },
        balloon,
        contentElement,
        originalDisplay,
        isVisible: false,
        showTimer: null,
        hideTimer: null,
        handlers: {},
        customStyleLink: null
    };

    // Load custom CSS if specified
    if (properties.balloonStyle === STYLE_CUSTOM && properties.customCssUrl) {
        loadCustomCss(state, properties.customCssUrl);
    }

    // Hide balloon initially
    balloon.style.display = "none";

    function positionBalloon() {
        const targetRect = target.getBoundingClientRect();
        const balloonRect = balloon.getBoundingClientRect();
        const scrollX = window.scrollX || window.pageXOffset;
        const scrollY = window.scrollY || window.pageYOffset;
        const offsetX = properties.offsetX || 0;
        const offsetY = properties.offsetY || 0;
        const arrowSize = 10;

        let position = properties.position;
        
        // Auto-detect best position
        if (position === POSITION_AUTO) {
            position = calculateAutoPosition(targetRect, balloonRect);
        }

        let left, top;
        const arrow = balloon.querySelector('.balloon-arrow');

        // Reset arrow classes
        arrow.className = 'balloon-arrow';

        switch (position) {
            case POSITION_TOP_LEFT:
                left = targetRect.left + scrollX + offsetX;
                top = targetRect.top - balloonRect.height - arrowSize + scrollY + offsetY;
                arrow.classList.add('arrow-bottom-left');
                break;
            case POSITION_TOP_RIGHT:
                left = targetRect.right - balloonRect.width + scrollX + offsetX;
                top = targetRect.top - balloonRect.height - arrowSize + scrollY + offsetY;
                arrow.classList.add('arrow-bottom-right');
                break;
            case POSITION_BOTTOM_LEFT:
                left = targetRect.left + scrollX + offsetX;
                top = targetRect.bottom + arrowSize + scrollY + offsetY;
                arrow.classList.add('arrow-top-left');
                break;
            case POSITION_BOTTOM_RIGHT:
            default:
                left = targetRect.right - balloonRect.width + scrollX + offsetX;
                top = targetRect.bottom + arrowSize + scrollY + offsetY;
                arrow.classList.add('arrow-top-right');
                break;
        }

        balloon.style.left = `${Math.max(0, left)}px`;
        balloon.style.top = `${Math.max(0, top)}px`;
    }

    function calculateAutoPosition(targetRect, balloonRect) {
        const viewportHeight = window.innerHeight;
        const viewportWidth = window.innerWidth;
        const arrowSize = 10;

        // Check if there's more space above or below
        const spaceAbove = targetRect.top;
        const spaceBelow = viewportHeight - targetRect.bottom;
        const preferTop = spaceAbove > spaceBelow && spaceAbove >= balloonRect.height + arrowSize;

        // Check if there's more space left or right
        const spaceLeft = targetRect.left;
        const spaceRight = viewportWidth - targetRect.right;
        const preferLeft = spaceLeft > spaceRight;

        if (preferTop) {
            return preferLeft ? POSITION_TOP_LEFT : POSITION_TOP_RIGHT;
        } else {
            return preferLeft ? POSITION_BOTTOM_LEFT : POSITION_BOTTOM_RIGHT;
        }
    }

    function show() {
        if (state.isVisible) return;
        state.isVisible = true;

        balloon.style.display = "";
        positionBalloon();

        // Add outside-click dismiss with a short delay
        requestAnimationFrame(() => {
            document.addEventListener("mousedown", state.handlers.outsideClick);
        });
    }

    function hide() {
        if (!state.isVisible) return;
        state.isVisible = false;

        balloon.style.display = "none";
        document.removeEventListener("mousedown", state.handlers.outsideClick);
    }

    function cancelTimers() {
        if (state.showTimer) {
            clearTimeout(state.showTimer);
            state.showTimer = null;
        }
        if (state.hideTimer) {
            clearTimeout(state.hideTimer);
            state.hideTimer = null;
        }
    }

    function scheduleShow() {
        cancelTimers();
        // Small delay for smoother UX
        state.showTimer = setTimeout(show, 100);
    }

    function scheduleHide() {
        if (state.showTimer) {
            clearTimeout(state.showTimer);
            state.showTimer = null;
        }
        // Delay to allow moving cursor to balloon
        state.hideTimer = setTimeout(hide, 200);
    }

    // Outside click handler
    state.handlers.outsideClick = function (e) {
        if (!balloon.contains(e.target) && !target.contains(e.target)) {
            hide();
        }
    };

    // Mouse hover handlers
    if (properties.displayOnMouseOver) {
        state.handlers.mouseEnter = function () {
            scheduleShow();
        };
        state.handlers.mouseLeave = function () {
            scheduleHide();
        };
        state.handlers.balloonMouseEnter = function () {
            cancelTimers();
        };
        state.handlers.balloonMouseLeave = function () {
            scheduleHide();
        };

        target.addEventListener("mouseenter", state.handlers.mouseEnter);
        target.addEventListener("mouseleave", state.handlers.mouseLeave);
        balloon.addEventListener("mouseenter", state.handlers.balloonMouseEnter);
        balloon.addEventListener("mouseleave", state.handlers.balloonMouseLeave);
    }

    // Focus handlers
    if (properties.displayOnFocus) {
        state.handlers.focus = function () {
            show();
        };
        state.handlers.blur = function () {
            // Delay to allow clicking within balloon
            scheduleHide();
        };

        target.addEventListener("focus", state.handlers.focus);
        target.addEventListener("blur", state.handlers.blur);
    }

    // Click handler
    if (properties.displayOnClick) {
        state.handlers.click = function (e) {
            e.preventDefault();
            if (state.isVisible) {
                hide();
            } else {
                show();
            }
        };

        target.addEventListener("click", state.handlers.click);
    }

    // Escape key closes balloon
    state.handlers.keydown = function (e) {
        if (e.key === "Escape" && state.isVisible) {
            hide();
        }
    };
    document.addEventListener("keydown", state.handlers.keydown);

    state.show = show;
    state.hide = hide;
    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Creates the balloon container with styling based on properties.
 */
function createBalloonContainer(properties) {
    const balloon = document.createElement('div');
    balloon.className = 'balloon-popup';
    balloon.style.cssText = `
        position: absolute;
        z-index: 99999;
    `;

    // Apply style-specific classes and styling
    applyBalloonStyle(balloon, properties);

    // Apply size
    const sizePreset = SIZE_PRESETS[properties.balloonSize] || SIZE_PRESETS[SIZE_MEDIUM];
    balloon.style.width = `${sizePreset.width}px`;
    balloon.style.maxHeight = `${sizePreset.maxHeight}px`;

    // Apply shadow
    if (properties.useShadow) {
        balloon.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.25)';
    }

    // Create content wrapper with scrollbar settings
    const contentWrapper = document.createElement('div');
    contentWrapper.className = 'balloon-content';
    contentWrapper.style.cssText = `
        padding: 10px;
        max-height: ${sizePreset.maxHeight - 20}px;
    `;
    applyScrollBars(contentWrapper, properties.scrollBars);

    // Create arrow element
    const arrow = document.createElement('div');
    arrow.className = 'balloon-arrow';
    applyArrowStyle(arrow, properties);

    balloon.appendChild(contentWrapper);
    balloon.appendChild(arrow);

    return balloon;
}

/**
 * Applies balloon visual style based on BalloonStyle property.
 */
function applyBalloonStyle(balloon, properties) {
    switch (properties.balloonStyle) {
        case STYLE_CLOUD:
            balloon.style.cssText += `
                background: #fff;
                border: 2px solid #666;
                border-radius: 20px;
            `;
            balloon.classList.add('balloon-style-cloud');
            break;
        case STYLE_CUSTOM:
            balloon.classList.add('balloon-style-custom');
            break;
        case STYLE_RECTANGLE:
        default:
            balloon.style.cssText += `
                background: #fff;
                border: 1px solid #ccc;
                border-radius: 4px;
            `;
            balloon.classList.add('balloon-style-rectangle');
            break;
    }
}

/**
 * Applies arrow styling based on balloon style.
 */
function applyArrowStyle(arrow, properties) {
    const arrowBaseStyle = `
        position: absolute;
        width: 0;
        height: 0;
    `;

    arrow.style.cssText = arrowBaseStyle;

    // Add CSS for arrow positions
    const style = document.createElement('style');
    style.textContent = `
        .balloon-arrow.arrow-top-left,
        .balloon-arrow.arrow-top-right {
            top: -10px;
            border-left: 10px solid transparent;
            border-right: 10px solid transparent;
            border-bottom: 10px solid ${properties.balloonStyle === STYLE_CLOUD ? '#666' : '#ccc'};
        }
        .balloon-arrow.arrow-top-left { left: 15px; }
        .balloon-arrow.arrow-top-right { right: 15px; }
        
        .balloon-arrow.arrow-bottom-left,
        .balloon-arrow.arrow-bottom-right {
            bottom: -10px;
            border-left: 10px solid transparent;
            border-right: 10px solid transparent;
            border-top: 10px solid ${properties.balloonStyle === STYLE_CLOUD ? '#666' : '#ccc'};
        }
        .balloon-arrow.arrow-bottom-left { left: 15px; }
        .balloon-arrow.arrow-bottom-right { right: 15px; }
    `;
    
    if (!document.getElementById('balloon-popup-arrow-styles')) {
        style.id = 'balloon-popup-arrow-styles';
        document.head.appendChild(style);
    }
}

/**
 * Applies scrollbar settings to the content wrapper.
 */
function applyScrollBars(element, scrollBars) {
    switch (scrollBars) {
        case SCROLLBARS_HORIZONTAL:
            element.style.overflowX = 'scroll';
            element.style.overflowY = 'hidden';
            break;
        case SCROLLBARS_VERTICAL:
            element.style.overflowX = 'hidden';
            element.style.overflowY = 'scroll';
            break;
        case SCROLLBARS_BOTH:
            element.style.overflow = 'scroll';
            break;
        case SCROLLBARS_AUTO:
            element.style.overflow = 'auto';
            break;
        case SCROLLBARS_NONE:
        default:
            element.style.overflow = 'hidden';
            break;
    }
}

/**
 * Loads custom CSS for the balloon popup.
 */
function loadCustomCss(state, url) {
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = url;
    document.head.appendChild(link);
    state.customStyleLink = link;
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

    // Cancel any pending timers
    if (state.showTimer) clearTimeout(state.showTimer);
    if (state.hideTimer) clearTimeout(state.hideTimer);

    // Hide if visible
    if (state.isVisible) {
        state.balloon.style.display = "none";
    }

    const target = document.getElementById(state.targetId);

    // Remove hover handlers
    if (state.handlers.mouseEnter && target) {
        target.removeEventListener("mouseenter", state.handlers.mouseEnter);
        target.removeEventListener("mouseleave", state.handlers.mouseLeave);
    }
    if (state.handlers.balloonMouseEnter) {
        state.balloon.removeEventListener("mouseenter", state.handlers.balloonMouseEnter);
        state.balloon.removeEventListener("mouseleave", state.handlers.balloonMouseLeave);
    }

    // Remove focus handlers
    if (state.handlers.focus && target) {
        target.removeEventListener("focus", state.handlers.focus);
        target.removeEventListener("blur", state.handlers.blur);
    }

    // Remove click handler
    if (state.handlers.click && target) {
        target.removeEventListener("click", state.handlers.click);
    }

    // Remove document listeners
    document.removeEventListener("keydown", state.handlers.keydown);
    document.removeEventListener("mousedown", state.handlers.outsideClick);

    // Restore content element to its original location
    if (state.contentElement && state.contentElement.parentElement) {
        state.contentElement.style.display = state.originalDisplay;
        document.body.appendChild(state.contentElement);
    }

    // Remove balloon from DOM
    if (state.balloon && state.balloon.parentElement) {
        state.balloon.parentElement.removeChild(state.balloon);
    }

    // Remove custom CSS link if added
    if (state.customStyleLink && state.customStyleLink.parentElement) {
        state.customStyleLink.parentElement.removeChild(state.customStyleLink);
    }

    behaviors.delete(behaviorId);
}
