// ValidatorCalloutExtender JS behavior module
// Enhances validators by showing validation messages in a callout/tooltip bubble.

const behaviors = new Map();

/**
 * Creates a validator callout behavior and attaches it to the target validator element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[ValidatorCalloutExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        target,
        calloutElement: null,
        associatedInput: null,
        observer: null,
        ...properties
    };

    // Find the associated input element (validator's controltovalidate)
    state.associatedInput = findAssociatedInput(target);

    // Create the callout element (hidden initially)
    state.calloutElement = createCalloutElement(state);
    document.body.appendChild(state.calloutElement);

    // Set up observer to watch for validation state changes
    state.observer = createValidationObserver(state);

    // Check initial state
    checkValidationState(state);

    behaviors.set(behaviorId, state);
    return {};
}

/**
 * Finds the input element associated with a validator.
 * @param {HTMLElement} validator - The validator element
 * @returns {HTMLElement|null} The associated input element
 */
function findAssociatedInput(validator) {
    // Try to find via controltovalidate attribute (ASP.NET pattern)
    const controlToValidate = validator.getAttribute('data-val-controltovalidate') ||
                              validator.getAttribute('controltovalidate');
    
    if (controlToValidate) {
        const input = document.getElementById(controlToValidate);
        if (input) return input;
    }

    // Try to find via aria-describedby relationship (reverse lookup)
    const validatorId = validator.id;
    if (validatorId) {
        const input = document.querySelector(`[aria-describedby*="${validatorId}"]`);
        if (input) return input;
    }

    // Try to find the previous input sibling
    let sibling = validator.previousElementSibling;
    while (sibling) {
        if (sibling.tagName === 'INPUT' || sibling.tagName === 'TEXTAREA' || sibling.tagName === 'SELECT') {
            return sibling;
        }
        sibling = sibling.previousElementSibling;
    }

    return null;
}

/**
 * Creates the callout popup element.
 * @param {object} state - The behavior state
 * @returns {HTMLElement} The callout element
 */
function createCalloutElement(state) {
    const callout = document.createElement('div');
    callout.className = 'validator-callout' + (state.cssClass ? ' ' + state.cssClass : '');
    callout.style.cssText = `
        position: absolute;
        display: none;
        width: ${state.width}px;
        background: #fff;
        border: 1px solid #c00;
        border-radius: 4px;
        padding: 8px 12px;
        box-shadow: 2px 2px 8px rgba(0,0,0,0.2);
        z-index: 10000;
        font-size: 12px;
        color: #c00;
    `;

    // Add arrow/pointer
    const arrow = document.createElement('div');
    arrow.className = 'validator-callout-arrow';
    arrow.style.cssText = `
        position: absolute;
        width: 0;
        height: 0;
        border: 8px solid transparent;
    `;
    callout.appendChild(arrow);

    // Content container
    const content = document.createElement('div');
    content.className = 'validator-callout-content';
    content.style.cssText = 'display: flex; align-items: flex-start; gap: 8px;';

    // Warning icon (if provided)
    if (state.warningIconImageUrl) {
        const icon = document.createElement('img');
        icon.src = state.warningIconImageUrl;
        icon.alt = 'Warning';
        icon.style.cssText = 'width: 16px; height: 16px; flex-shrink: 0;';
        content.appendChild(icon);
    }

    // Message container
    const message = document.createElement('span');
    message.className = 'validator-callout-message';
    message.style.cssText = 'flex: 1;';
    content.appendChild(message);

    // Close button (if close image provided)
    if (state.closeImageUrl) {
        const closeBtn = document.createElement('img');
        closeBtn.src = state.closeImageUrl;
        closeBtn.alt = 'Close';
        closeBtn.style.cssText = 'width: 12px; height: 12px; cursor: pointer; flex-shrink: 0;';
        closeBtn.addEventListener('click', () => hideCallout(state));
        content.appendChild(closeBtn);
    }

    callout.appendChild(content);
    return callout;
}

/**
 * Creates a MutationObserver to watch for validation state changes.
 * @param {object} state - The behavior state
 * @returns {MutationObserver} The observer instance
 */
function createValidationObserver(state) {
    const observer = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === 'attributes' || mutation.type === 'childList') {
                checkValidationState(state);
            }
        }
    });

    // Observe the validator element for changes
    observer.observe(state.target, {
        attributes: true,
        attributeFilter: ['style', 'class', 'hidden'],
        childList: true,
        characterData: true,
        subtree: true
    });

    // Also observe parent in case validator is shown/hidden via parent
    if (state.target.parentElement) {
        observer.observe(state.target.parentElement, {
            childList: true
        });
    }

    return observer;
}

/**
 * Checks the validation state and shows/hides the callout accordingly.
 * @param {object} state - The behavior state
 */
function checkValidationState(state) {
    const validator = state.target;
    const errorMessage = getErrorMessage(validator);
    const isInvalid = isValidatorShowing(validator) && errorMessage;

    if (isInvalid) {
        showCallout(state, errorMessage);
    } else {
        hideCallout(state);
    }
}

/**
 * Determines if a validator is currently showing an error.
 * @param {HTMLElement} validator - The validator element
 * @returns {boolean} True if the validator is showing an error
 */
function isValidatorShowing(validator) {
    // Check various ways a validator might indicate it's active
    const style = window.getComputedStyle(validator);
    
    // Hidden via display or visibility
    if (style.display === 'none' || style.visibility === 'hidden') {
        return false;
    }

    // Check for ASP.NET validator visibility attribute
    if (validator.style.visibility === 'hidden' || validator.style.display === 'none') {
        return false;
    }

    // Check for hidden attribute
    if (validator.hidden) {
        return false;
    }

    // Check for aria-invalid on the element itself
    if (validator.getAttribute('aria-hidden') === 'true') {
        return false;
    }

    // If it has text content or inner HTML, it's likely showing
    const text = validator.textContent || validator.innerText;
    return text && text.trim().length > 0;
}

/**
 * Gets the error message from a validator element.
 * @param {HTMLElement} validator - The validator element
 * @returns {string} The error message
 */
function getErrorMessage(validator) {
    // Try innerText first (most common)
    const text = (validator.textContent || validator.innerText || '').trim();
    if (text) return text;

    // Try data-errormessage attribute
    const dataMsg = validator.getAttribute('data-errormessage') || 
                    validator.getAttribute('errormessage');
    if (dataMsg) return dataMsg;

    // Try title attribute
    if (validator.title) return validator.title;

    return '';
}

/**
 * Shows the callout with the error message.
 * @param {object} state - The behavior state
 * @param {string} message - The error message to display
 */
function showCallout(state, message) {
    const callout = state.calloutElement;
    const messageEl = callout.querySelector('.validator-callout-message');
    
    if (messageEl) {
        messageEl.textContent = message;
    }

    // Apply highlight class to the input
    if (state.associatedInput && state.highlightCssClass) {
        state.associatedInput.classList.add(state.highlightCssClass);
    }

    // Position the callout
    positionCallout(state);

    callout.style.display = 'block';
}

/**
 * Hides the callout.
 * @param {object} state - The behavior state
 */
function hideCallout(state) {
    state.calloutElement.style.display = 'none';

    // Remove highlight class from the input
    if (state.associatedInput && state.highlightCssClass) {
        state.associatedInput.classList.remove(state.highlightCssClass);
    }
}

/**
 * Positions the callout relative to the associated input or validator.
 * @param {object} state - The behavior state
 */
function positionCallout(state) {
    const callout = state.calloutElement;
    const arrow = callout.querySelector('.validator-callout-arrow');
    const reference = state.associatedInput || state.target;
    
    if (!reference) return;

    const refRect = reference.getBoundingClientRect();
    const calloutRect = callout.getBoundingClientRect();
    const scrollX = window.pageXOffset || document.documentElement.scrollLeft;
    const scrollY = window.pageYOffset || document.documentElement.scrollTop;

    let left, top;
    const arrowSize = 8;
    const gap = 4;

    // Reset arrow styles
    arrow.style.cssText = `
        position: absolute;
        width: 0;
        height: 0;
        border: ${arrowSize}px solid transparent;
    `;

    switch (state.popupPosition) {
        case 'TopLeft':
            left = refRect.left + scrollX;
            top = refRect.top + scrollY - calloutRect.height - arrowSize - gap;
            arrow.style.bottom = `-${arrowSize * 2}px`;
            arrow.style.left = '20px';
            arrow.style.borderTopColor = '#c00';
            break;
        case 'TopRight':
            left = refRect.right + scrollX - state.width;
            top = refRect.top + scrollY - calloutRect.height - arrowSize - gap;
            arrow.style.bottom = `-${arrowSize * 2}px`;
            arrow.style.right = '20px';
            arrow.style.borderTopColor = '#c00';
            break;
        case 'Top':
            left = refRect.left + scrollX + (refRect.width / 2) - (state.width / 2);
            top = refRect.top + scrollY - calloutRect.height - arrowSize - gap;
            arrow.style.bottom = `-${arrowSize * 2}px`;
            arrow.style.left = '50%';
            arrow.style.transform = 'translateX(-50%)';
            arrow.style.borderTopColor = '#c00';
            break;
        case 'BottomRight':
            left = refRect.right + scrollX - state.width;
            top = refRect.bottom + scrollY + arrowSize + gap;
            arrow.style.top = `-${arrowSize * 2}px`;
            arrow.style.right = '20px';
            arrow.style.borderBottomColor = '#c00';
            break;
        case 'Bottom':
            left = refRect.left + scrollX + (refRect.width / 2) - (state.width / 2);
            top = refRect.bottom + scrollY + arrowSize + gap;
            arrow.style.top = `-${arrowSize * 2}px`;
            arrow.style.left = '50%';
            arrow.style.transform = 'translateX(-50%)';
            arrow.style.borderBottomColor = '#c00';
            break;
        case 'Left':
            left = refRect.left + scrollX - state.width - arrowSize - gap;
            top = refRect.top + scrollY + (refRect.height / 2) - (calloutRect.height / 2);
            arrow.style.right = `-${arrowSize * 2}px`;
            arrow.style.top = '50%';
            arrow.style.transform = 'translateY(-50%)';
            arrow.style.borderLeftColor = '#c00';
            break;
        case 'Right':
            left = refRect.right + scrollX + arrowSize + gap;
            top = refRect.top + scrollY + (refRect.height / 2) - (calloutRect.height / 2);
            arrow.style.left = `-${arrowSize * 2}px`;
            arrow.style.top = '50%';
            arrow.style.transform = 'translateY(-50%)';
            arrow.style.borderRightColor = '#c00';
            break;
        case 'BottomLeft':
        default:
            left = refRect.left + scrollX;
            top = refRect.bottom + scrollY + arrowSize + gap;
            arrow.style.top = `-${arrowSize * 2}px`;
            arrow.style.left = '20px';
            arrow.style.borderBottomColor = '#c00';
            break;
    }

    // Ensure callout stays within viewport
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    if (left < scrollX + 10) {
        left = scrollX + 10;
    } else if (left + state.width > scrollX + viewportWidth - 10) {
        left = scrollX + viewportWidth - state.width - 10;
    }

    if (top < scrollY + 10) {
        top = scrollY + 10;
    }

    callout.style.left = left + 'px';
    callout.style.top = top + 'px';
}

/**
 * Updates behavior properties.
 * @param {string} behaviorId
 * @param {object} properties
 */
export function updateBehavior(behaviorId, properties) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Update properties
    Object.assign(state, properties);

    // Update callout width
    if (state.calloutElement) {
        state.calloutElement.style.width = state.width + 'px';
    }

    // Re-check validation state with new properties
    checkValidationState(state);
}

/**
 * Disposes the behavior and removes the callout element.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    // Disconnect observer
    if (state.observer) {
        state.observer.disconnect();
    }

    // Remove highlight class from input
    if (state.associatedInput && state.highlightCssClass) {
        state.associatedInput.classList.remove(state.highlightCssClass);
    }

    // Remove callout element from DOM
    if (state.calloutElement && state.calloutElement.parentNode) {
        state.calloutElement.parentNode.removeChild(state.calloutElement);
    }

    behaviors.delete(behaviorId);
}
