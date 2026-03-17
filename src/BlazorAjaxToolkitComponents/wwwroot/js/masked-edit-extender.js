// MaskedEditExtender JS behavior module
// Applies an input mask to a target input, restricting and formatting keystrokes.

const behaviors = new Map();

// MaskType (matches C# enum)
const MASK_NONE = 0;
const MASK_NUMBER = 1;
const MASK_DATE = 2;
const MASK_TIME = 3;
const MASK_DATETIME = 4;

// InputDirection (matches C# enum)
const DIR_LTR = 0;
const DIR_RTL = 1;

// AcceptNegative (matches C# enum)
const NEG_NONE = 0;
const NEG_LEFT = 1;
const NEG_RIGHT = 2;

// DisplayMoney (matches C# enum)
const MONEY_NONE = 0;
const MONEY_LEFT = 1;
const MONEY_RIGHT = 2;

// Mask legend: 9=digit, L=letter, $=letter or space, C=any char, A=alphanumeric, N=number or space, ?=any char or space
function getMaskSlots(mask) {
    const slots = [];
    for (let i = 0; i < mask.length; i++) {
        const ch = mask[i];
        if (ch === "9" || ch === "L" || ch === "$" || ch === "C" || ch === "A" || ch === "N" || ch === "?") {
            slots.push({ index: i, type: ch });
        }
    }
    return slots;
}

function isValidForSlot(ch, slotType, filtered) {
    // Check if character is in the filtered extra characters
    if (filtered && filtered.indexOf(ch) >= 0) {
        return true;
    }
    switch (slotType) {
        case "9": return /\d/.test(ch);
        case "L": return /[a-zA-Z]/.test(ch);
        case "$": return /[a-zA-Z ]/.test(ch);
        case "C": return true;
        case "A": return /[a-zA-Z0-9]/.test(ch);
        case "N": return /[\d ]/.test(ch);
        case "?": return true;
        default: return false;
    }
}

/**
 * Creates a masked edit behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[MaskedEditExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const mask = properties.mask || "";
    const promptChar = properties.promptCharacter || "_";
    const slots = getMaskSlots(mask);

    const state = {
        targetId,
        properties: { ...properties },
        mask,
        promptChar,
        slots,
        handlers: {}
    };

    function buildDisplayFromMask(values) {
        let display = "";
        let slotIdx = 0;
        for (let i = 0; i < mask.length; i++) {
            if (slotIdx < slots.length && slots[slotIdx].index === i) {
                display += values[slotIdx] !== undefined ? values[slotIdx] : promptChar;
                slotIdx++;
            } else {
                display += mask[i];
            }
        }
        return display;
    }

    function getSlotValues() {
        const val = target.value;
        const values = [];
        let slotIdx = 0;
        for (let i = 0; i < mask.length && i < val.length; i++) {
            if (slotIdx < slots.length && slots[slotIdx].index === i) {
                const ch = val[i];
                values.push(ch === promptChar ? undefined : ch);
                slotIdx++;
            }
        }
        return values;
    }

    function findNextEditablePosition(startSlotIdx) {
        if (startSlotIdx < slots.length) return startSlotIdx;
        return -1;
    }

    function getSlotIndexAtCursor(cursorPos) {
        for (let i = 0; i < slots.length; i++) {
            if (slots[i].index >= cursorPos) return i;
        }
        return slots.length;
    }

    function setCursorAt(pos) {
        requestAnimationFrame(() => {
            target.setSelectionRange(pos, pos);
        });
    }

    // Initialize with mask display
    if (mask && !target.value) {
        target.value = buildDisplayFromMask([]);
    }

    state.handlers.keydown = function (e) {
        if (!mask) return;
        if (e.ctrlKey || e.metaKey || e.altKey) return;

        const cursorPos = target.selectionStart;

        if (e.key === "Backspace") {
            e.preventDefault();
            const values = getSlotValues();
            let slotIdx = getSlotIndexAtCursor(cursorPos) - 1;
            if (slotIdx >= 0 && slotIdx < slots.length) {
                values[slotIdx] = undefined;
                target.value = buildDisplayFromMask(values);
                setCursorAt(slots[slotIdx].index);
            }
            return;
        }

        if (e.key === "Delete") {
            e.preventDefault();
            const values = getSlotValues();
            let slotIdx = getSlotIndexAtCursor(cursorPos);
            if (slotIdx >= 0 && slotIdx < slots.length) {
                values[slotIdx] = undefined;
                target.value = buildDisplayFromMask(values);
                setCursorAt(slots[slotIdx].index);
            }
            return;
        }

        // Arrow keys, Tab, etc. — let through
        if (e.key.length > 1) return;

        e.preventDefault();
        const ch = e.key;
        const values = getSlotValues();
        let slotIdx = getSlotIndexAtCursor(cursorPos);
        slotIdx = findNextEditablePosition(slotIdx);

        if (slotIdx < 0 || slotIdx >= slots.length) return;

        if (!isValidForSlot(ch, slots[slotIdx].type, properties.filtered)) {
            if (properties.errorTooltipEnabled) {
                target.title = `Invalid character for position ${slotIdx + 1}`;
            }
            return;
        }

        values[slotIdx] = ch;
        target.value = buildDisplayFromMask(values);
        target.dispatchEvent(new Event("input", { bubbles: true }));

        // Move cursor to next editable slot
        const nextSlot = findNextEditablePosition(slotIdx + 1);
        if (nextSlot >= 0 && nextSlot < slots.length) {
            setCursorAt(slots[nextSlot].index);
        } else {
            setCursorAt(slots[slotIdx].index + 1);
        }
    };

    state.handlers.focus = function () {
        if (!target.value || target.value.length === 0) {
            target.value = buildDisplayFromMask([]);
        }
        // Apply focus CSS class
        if (properties.onFocusCssClass) {
            target.classList.add(properties.onFocusCssClass);
        }
        // Position cursor at first empty slot
        const values = getSlotValues();
        for (let i = 0; i < slots.length; i++) {
            if (values[i] === undefined) {
                setCursorAt(slots[i].index);
                return;
            }
        }
    };

    state.handlers.blur = function () {
        // Remove focus CSS class
        if (properties.onFocusCssClass) {
            target.classList.remove(properties.onFocusCssClass);
        }
        // Remove invalid CSS class initially (will re-add if needed)
        if (properties.onInvalidCssClass) {
            target.classList.remove(properties.onInvalidCssClass);
        }

        const values = getSlotValues();
        const hasValue = values.some(v => v !== undefined);
        const allFilled = values.every(v => v !== undefined);

        if (properties.clearTextOnInvalid) {
            if (!allFilled && hasValue) {
                target.value = "";
                target.dispatchEvent(new Event("input", { bubbles: true }));
                if (properties.onInvalidCssClass) {
                    target.classList.add(properties.onInvalidCssClass);
                }
                return;
            }
        }

        // Check for negative value and apply CSS class
        if (properties.onBlurCssNegative && hasValue) {
            const rawValue = values.filter(v => v !== undefined).join("");
            if (rawValue.indexOf("-") >= 0 || (properties.acceptNegative !== NEG_NONE && rawValue.startsWith("-"))) {
                target.classList.add(properties.onBlurCssNegative);
            } else {
                target.classList.remove(properties.onBlurCssNegative);
            }
        }

        if (properties.clearMaskOnLostFocus && hasValue) {
            const rawValue = values.filter(v => v !== undefined).join("");
            target.value = rawValue;
            target.dispatchEvent(new Event("input", { bubbles: true }));
        } else if (!hasValue) {
            target.value = "";
            target.dispatchEvent(new Event("input", { bubbles: true }));
        }
    };

    // Prevent paste from bypassing mask
    state.handlers.paste = function (e) {
        e.preventDefault();
        const text = (e.clipboardData || window.clipboardData).getData("text");
        if (!text) return;

        const values = getSlotValues();
        let slotIdx = getSlotIndexAtCursor(target.selectionStart);
        for (let i = 0; i < text.length && slotIdx < slots.length; i++) {
            if (isValidForSlot(text[i], slots[slotIdx].type, properties.filtered)) {
                values[slotIdx] = text[i];
                slotIdx++;
            }
        }
        target.value = buildDisplayFromMask(values);
        target.dispatchEvent(new Event("input", { bubbles: true }));

        if (slotIdx < slots.length) {
            setCursorAt(slots[slotIdx].index);
        }
    };

    target.addEventListener("keydown", state.handlers.keydown);
    target.addEventListener("focus", state.handlers.focus);
    target.addEventListener("blur", state.handlers.blur);
    target.addEventListener("paste", state.handlers.paste);

    // Disable native autocomplete if not enabled
    if (!properties.autoComplete) {
        target.setAttribute("autocomplete", "off");
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
    if (target) {
        target.removeEventListener("keydown", state.handlers.keydown);
        target.removeEventListener("focus", state.handlers.focus);
        target.removeEventListener("blur", state.handlers.blur);
        target.removeEventListener("paste", state.handlers.paste);
    }

    behaviors.delete(behaviorId);
}
