// TabContainer JS behavior module
// Provides client-side callback support for tab change events.

/**
 * Invokes a named client-side JavaScript function when the active tab changes.
 * @param {string} callbackName - The name of the global JS function to invoke.
 * @param {number} tabIndex - The new active tab index.
 */
export function invokeClientCallback(callbackName, tabIndex) {
    if (!callbackName) return;

    try {
        const fn = resolveFunction(callbackName);
        if (typeof fn === "function") {
            fn(tabIndex);
        } else {
            console.warn(`[TabContainer] OnClientActiveTabChanged: '${callbackName}' is not a function.`);
        }
    } catch (ex) {
        console.warn(`[TabContainer] Error invoking '${callbackName}':`, ex);
    }
}

/**
 * Resolves a dotted function name to a callable reference.
 * Supports global functions and nested paths (e.g., "MyApp.onTabChanged").
 * @param {string} name
 * @returns {Function|null}
 */
function resolveFunction(name) {
    const parts = name.split(".");
    let current = window;
    for (const part of parts) {
        current = current[part];
        if (current == null) return null;
    }
    return current;
}
