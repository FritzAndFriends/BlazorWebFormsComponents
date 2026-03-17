// AutoCompleteExtender JS behavior module
// Provides typeahead/autocomplete dropdown for a target input element.

const behaviors = new Map();

/**
 * Creates an autocomplete behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[AutoCompleteExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const state = {
        targetId,
        properties: { ...properties },
        dropdown: null,
        items: [],
        highlightedIndex: -1,
        debounceTimer: null,
        isOpen: false,
        handlers: {}
    };

    // Create dropdown container
    const dropdown = document.createElement("div");
    dropdown.style.position = "absolute";
    dropdown.style.zIndex = "10000";
    dropdown.style.display = "none";
    dropdown.style.backgroundColor = "#fff";
    dropdown.style.border = "1px solid #ccc";
    dropdown.style.boxShadow = "0 2px 8px rgba(0,0,0,0.15)";
    dropdown.style.maxHeight = "200px";
    dropdown.style.overflowY = "auto";
    if (properties.completionListCssClass) {
        dropdown.className = properties.completionListCssClass;
    }
    document.body.appendChild(dropdown);
    state.dropdown = dropdown;

    function getDelimiters() {
        return (properties.delimiterCharacters || "").split("");
    }

    function getCurrentWord() {
        const val = target.value;
        const delimiters = getDelimiters();
        if (!delimiters.length || !delimiters[0]) return val;
        const pos = target.selectionStart || val.length;
        let start = 0;
        for (let i = pos - 1; i >= 0; i--) {
            if (delimiters.includes(val[i])) {
                start = i + 1;
                break;
            }
        }
        return val.substring(start, pos).trimStart();
    }

    function positionDropdown() {
        const rect = target.getBoundingClientRect();
        dropdown.style.top = `${rect.bottom + window.scrollY}px`;
        dropdown.style.left = `${rect.left + window.scrollX}px`;
        dropdown.style.minWidth = `${rect.width}px`;
    }

    function renderItems(items) {
        state.items = items || [];
        state.highlightedIndex = properties.firstRowSelected && items.length > 0 ? 0 : -1;

        if (!items || items.length === 0) {
            closeDropdown();
            return;
        }

        const itemCss = properties.completionListItemCssClass || "";
        const highlightCss = properties.completionListHighlightedItemCssClass || "";

        let html = "";
        const maxCount = Math.min(items.length, properties.completionSetCount || 10);
        for (let i = 0; i < maxCount; i++) {
            const displayText = properties.showOnlyCurrentWordInCompletionListItem
                ? items[i]
                : items[i];
            const isHighlighted = i === state.highlightedIndex;
            const cls = isHighlighted ? highlightCss : itemCss;
            const bgStyle = isHighlighted ? "background:#0078d4;color:#fff;" : "";
            html += `<div data-index="${i}" class="${cls}" style="padding:4px 8px;cursor:pointer;font-size:13px;${bgStyle}">${escapeHtml(displayText)}</div>`;
        }
        dropdown.innerHTML = html;
        positionDropdown();
        dropdown.style.display = "block";
        state.isOpen = true;

        if (properties.onClientPopulated) {
            try { new Function(properties.onClientPopulated)(); } catch (_) { }
        }
    }

    function escapeHtml(text) {
        const div = document.createElement("div");
        div.textContent = text;
        return div.innerHTML;
    }

    function closeDropdown() {
        dropdown.style.display = "none";
        state.isOpen = false;
        state.highlightedIndex = -1;
    }

    function selectItem(index) {
        if (index < 0 || index >= state.items.length) return;
        const value = state.items[index];
        const delimiters = getDelimiters();
        if (delimiters.length && delimiters[0]) {
            const val = target.value;
            const pos = target.selectionStart || val.length;
            let start = 0;
            for (let i = pos - 1; i >= 0; i--) {
                if (delimiters.includes(val[i])) {
                    start = i + 1;
                    break;
                }
            }
            target.value = val.substring(0, start) + value + val.substring(pos);
        } else {
            target.value = value;
        }
        target.dispatchEvent(new Event("input", { bubbles: true }));
        target.dispatchEvent(new Event("change", { bubbles: true }));
        closeDropdown();

        if (properties.onClientItemSelected) {
            try { new Function(properties.onClientItemSelected)(); } catch (_) { }
        }
    }

    function updateHighlight(newIndex) {
        const children = dropdown.children;
        const highlightCss = properties.completionListHighlightedItemCssClass || "";
        const itemCss = properties.completionListItemCssClass || "";

        if (state.highlightedIndex >= 0 && state.highlightedIndex < children.length) {
            children[state.highlightedIndex].className = itemCss;
            children[state.highlightedIndex].style.background = "";
            children[state.highlightedIndex].style.color = "";
        }
        state.highlightedIndex = newIndex;
        if (newIndex >= 0 && newIndex < children.length) {
            children[newIndex].className = highlightCss;
            children[newIndex].style.background = "#0078d4";
            children[newIndex].style.color = "#fff";
            children[newIndex].scrollIntoView({ block: "nearest" });
        }
    }

    async function fetchCompletions(prefix) {
        if (properties.onClientPopulating) {
            try { new Function(properties.onClientPopulating)(); } catch (_) { }
        }

        // If servicePath and serviceMethod are provided, fetch from service
        if (properties.servicePath && properties.serviceMethod) {
            try {
                const url = `${properties.servicePath}/${properties.serviceMethod}`;
                const response = await fetch(url, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        prefixText: prefix,
                        count: properties.completionSetCount || 10
                    })
                });
                if (response.ok) {
                    const data = await response.json();
                    renderItems(Array.isArray(data) ? data : data.d || []);
                }
            } catch (err) {
                console.warn(`[AutoCompleteExtender] Service call failed:`, err);
            }
        }
    }

    // Input handler with debounce
    state.handlers.input = function () {
        clearTimeout(state.debounceTimer);
        const prefix = getCurrentWord();
        const minLen = properties.minimumPrefixLength || 3;
        if (prefix.length < minLen) {
            closeDropdown();
            return;
        }
        const interval = properties.completionInterval || 1000;
        state.debounceTimer = setTimeout(() => fetchCompletions(prefix), interval);
    };
    target.addEventListener("input", state.handlers.input);

    // Keyboard navigation
    state.handlers.keydown = function (e) {
        if (!state.isOpen) return;
        const maxIndex = Math.min(state.items.length, properties.completionSetCount || 10) - 1;
        switch (e.key) {
            case "ArrowDown":
                e.preventDefault();
                updateHighlight(Math.min(state.highlightedIndex + 1, maxIndex));
                break;
            case "ArrowUp":
                e.preventDefault();
                updateHighlight(Math.max(state.highlightedIndex - 1, 0));
                break;
            case "Enter":
                if (state.highlightedIndex >= 0) {
                    e.preventDefault();
                    selectItem(state.highlightedIndex);
                }
                break;
            case "Escape":
                closeDropdown();
                break;
        }
    };
    target.addEventListener("keydown", state.handlers.keydown);

    // Dropdown click delegation
    state.handlers.dropdownClick = function (e) {
        const item = e.target.closest("[data-index]");
        if (item) {
            selectItem(parseInt(item.dataset.index, 10));
        }
    };
    dropdown.addEventListener("click", state.handlers.dropdownClick);

    // Close on outside click
    state.handlers.documentClick = function (e) {
        if (!state.isOpen) return;
        if (!dropdown.contains(e.target) && e.target !== target) {
            closeDropdown();
        }
    };
    document.addEventListener("mousedown", state.handlers.documentClick);

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

    clearTimeout(state.debounceTimer);

    const target = document.getElementById(state.targetId);
    if (target) {
        target.removeEventListener("input", state.handlers.input);
        target.removeEventListener("keydown", state.handlers.keydown);
    }
    document.removeEventListener("mousedown", state.handlers.documentClick);

    if (state.dropdown) {
        state.dropdown.removeEventListener("click", state.handlers.dropdownClick);
        state.dropdown.remove();
    }

    behaviors.delete(behaviorId);
}
