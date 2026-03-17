// ListSearchExtender JS behavior module
// Enables search/filter functionality on a target select element (ListBox or DropDownList).

const behaviors = new Map();

/**
 * Creates a list search behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target || target.tagName !== "SELECT") {
        console.warn(`[ListSearchExtender] Target element '${targetId}' not found or is not a SELECT element.`);
        return {};
    }

    const { promptText, promptCssClass, promptPosition, isSorted, queryPattern } = properties;

    // Create the search input container
    const container = document.createElement("div");
    container.className = "list-search-container";
    container.style.display = "flex";
    container.style.flexDirection = "column";

    // Create the search input
    const searchInput = document.createElement("input");
    searchInput.type = "text";
    searchInput.placeholder = promptText;
    if (promptCssClass) {
        searchInput.className = promptCssClass;
    }
    searchInput.setAttribute("data-list-search-input", behaviorId);

    // Store original options for filtering
    const originalOptions = Array.from(target.options).map(opt => ({
        value: opt.value,
        text: opt.text,
        selected: opt.selected
    }));

    // Insert the search input relative to the target
    const parent = target.parentNode;
    if (promptPosition === "bottom") {
        // Insert container after target, with target first then input
        parent.insertBefore(container, target.nextSibling);
        container.appendChild(target);
        container.appendChild(searchInput);
    } else {
        // Default: top - insert container before target, with input first then target
        parent.insertBefore(container, target);
        container.appendChild(searchInput);
        container.appendChild(target);
    }

    /**
     * Filters options based on search text.
     * @param {string} searchText - The text to search for.
     */
    function filterOptions(searchText) {
        const lowerSearch = searchText.toLowerCase();

        if (searchText === "") {
            // Restore all original options
            restoreAllOptions();
            return;
        }

        // Clear current options
        target.innerHTML = "";

        // Find matching options
        const matchingOptions = originalOptions.filter(opt => {
            const lowerText = opt.text.toLowerCase();
            if (queryPattern === "startswith") {
                return lowerText.startsWith(lowerSearch);
            } else {
                return lowerText.includes(lowerSearch);
            }
        });

        // Add matching options back
        matchingOptions.forEach((opt, index) => {
            const option = document.createElement("option");
            option.value = opt.value;
            option.text = opt.text;
            if (index === 0) {
                option.selected = true;
            }
            target.appendChild(option);
        });

        // If using sorted list with StartsWith, we could use binary search
        // but for simplicity, linear search works for most use cases
    }

    /**
     * Restores all original options to the select element.
     */
    function restoreAllOptions() {
        target.innerHTML = "";
        originalOptions.forEach(opt => {
            const option = document.createElement("option");
            option.value = opt.value;
            option.text = opt.text;
            option.selected = opt.selected;
            target.appendChild(option);
        });
    }

    /**
     * Input handler for search field.
     */
    function inputHandler() {
        filterOptions(searchInput.value);
    }

    /**
     * Keydown handler for keyboard navigation.
     */
    function keydownHandler(e) {
        if (e.key === "Escape") {
            searchInput.value = "";
            restoreAllOptions();
            searchInput.blur();
        } else if (e.key === "Enter") {
            // Focus the select element to allow selection
            target.focus();
        } else if (e.key === "ArrowDown" || e.key === "ArrowUp") {
            // Let the select handle arrow keys
            target.focus();
        }
    }

    // Attach event listeners
    searchInput.addEventListener("input", inputHandler);
    searchInput.addEventListener("keydown", keydownHandler);

    const state = {
        targetId,
        container,
        searchInput,
        originalOptions,
        promptText,
        promptCssClass,
        promptPosition,
        isSorted,
        queryPattern,
        inputHandler,
        keydownHandler,
        filterOptions,
        restoreAllOptions
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

    const { promptText, promptCssClass, queryPattern, isSorted } = properties;

    // Update search input placeholder
    state.searchInput.placeholder = promptText;
    state.promptText = promptText;

    // Update CSS class
    if (promptCssClass !== state.promptCssClass) {
        state.searchInput.className = promptCssClass || "";
        state.promptCssClass = promptCssClass;
    }

    // Update search behavior
    state.queryPattern = queryPattern;
    state.isSorted = isSorted;

    // Re-filter with current search text
    if (state.searchInput.value) {
        state.filterOptions(state.searchInput.value);
    }
}

/**
 * Disposes the behavior and removes event listeners.
 * @param {string} behaviorId
 */
export function disposeBehavior(behaviorId) {
    const state = behaviors.get(behaviorId);
    if (!state) return;

    const target = document.getElementById(state.targetId);

    // Remove event listeners
    state.searchInput.removeEventListener("input", state.inputHandler);
    state.searchInput.removeEventListener("keydown", state.keydownHandler);

    // Restore original DOM structure
    if (state.container && state.container.parentNode) {
        const parent = state.container.parentNode;

        // Restore all original options
        if (target) {
            state.restoreAllOptions();
            // Move target back to its original position
            parent.insertBefore(target, state.container);
        }

        // Remove the container (which includes the search input)
        state.container.remove();
    }

    behaviors.delete(behaviorId);
}
