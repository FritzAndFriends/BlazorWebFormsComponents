// FilteredTextBoxExtender JS behavior module
// Restricts input in a target text field based on configurable filter rules.

const behaviors = new Map();

// FilterType flags (matches C# enum)
const FILTER_NUMBERS = 1;
const FILTER_LOWERCASE = 2;
const FILTER_UPPERCASE = 4;

// FilterMode (matches C# enum)
const MODE_VALID_CHARS = 0;
const MODE_INVALID_CHARS = 1;

/**
 * Builds a regex character class from the filter configuration.
 * @param {{ filterType: number, validChars: string, invalidChars: string, filterMode: number }} props
 * @returns {{ isAllowed: (char: string) => boolean }}
 */
function buildFilter(props) {
    const { filterType, validChars, invalidChars, filterMode } = props;

    // Build set of allowed characters from FilterType flags
    let allowedPattern = "";
    if (filterType & FILTER_NUMBERS) allowedPattern += "0-9";
    if (filterType & FILTER_LOWERCASE) allowedPattern += "a-z";
    if (filterType & FILTER_UPPERCASE) allowedPattern += "A-Z";

    // Escape special regex chars in user-provided strings
    const escapeRegex = (s) => s.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");

    if (filterMode === MODE_INVALID_CHARS && invalidChars) {
        // Block specific characters
        const invalidRegex = new RegExp(`[${escapeRegex(invalidChars)}]`);
        return {
            isAllowed: (ch) => !invalidRegex.test(ch)
        };
    }

    // ValidChars mode: build allowed set from filterType + validChars
    let pattern = allowedPattern;
    if (validChars) {
        pattern += escapeRegex(validChars);
    }

    if (!pattern) {
        // No filter configured — allow everything
        return { isAllowed: () => true };
    }

    const allowedRegex = new RegExp(`^[${pattern}]$`);
    return {
        isAllowed: (ch) => allowedRegex.test(ch)
    };
}

/**
 * Creates a filter behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[FilteredTextBoxExtender] Target element '${targetId}' not found.`);
        return {};
    }

    let filter = buildFilter(properties);
    let filterInterval = properties.filterInterval || 250;
    let filterTimer = null;

    function keypressHandler(e) {
        // Allow control keys
        if (e.ctrlKey || e.altKey || e.metaKey) return;
        if (e.key.length !== 1) return; // non-printable

        if (!filter.isAllowed(e.key)) {
            e.preventDefault();
        }
    }

    function inputHandler() {
        // Debounced cleanup for paste and other programmatic input
        if (filterTimer) clearTimeout(filterTimer);
        filterTimer = setTimeout(() => {
            const original = target.value;
            const filtered = Array.from(original).filter(ch => filter.isAllowed(ch)).join("");
            if (filtered !== original) {
                target.value = filtered;
                // Dispatch input event so Blazor picks up the change
                target.dispatchEvent(new Event("input", { bubbles: true }));
            }
        }, filterInterval);
    }

    function pasteHandler(e) {
        const paste = (e.clipboardData || window.clipboardData)?.getData("text") || "";
        const filtered = Array.from(paste).filter(ch => filter.isAllowed(ch)).join("");

        if (filtered !== paste) {
            e.preventDefault();
            // Insert filtered text at cursor position
            const start = target.selectionStart;
            const end = target.selectionEnd;
            const before = target.value.substring(0, start);
            const after = target.value.substring(end);
            target.value = before + filtered + after;
            target.selectionStart = target.selectionEnd = start + filtered.length;
            target.dispatchEvent(new Event("input", { bubbles: true }));
        }
    }

    target.addEventListener("keypress", keypressHandler);
    target.addEventListener("input", inputHandler);
    target.addEventListener("paste", pasteHandler);

    const state = {
        targetId,
        filter,
        filterInterval,
        filterTimer,
        keypressHandler,
        inputHandler,
        pasteHandler,
        properties: { ...properties }
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

    state.filter = buildFilter(properties);
    state.filterInterval = properties.filterInterval || 250;
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
    if (target) {
        target.removeEventListener("keypress", state.keypressHandler);
        target.removeEventListener("input", state.inputHandler);
        target.removeEventListener("paste", state.pasteHandler);
    }

    if (state.filterTimer) clearTimeout(state.filterTimer);
    behaviors.delete(behaviorId);
}
