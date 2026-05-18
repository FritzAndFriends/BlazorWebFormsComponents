// clientscript-demo.js — Blazor equivalents of Web Forms ClientScript patterns

/**
 * Web Forms: Page.ClientScript.RegisterStartupScript(...)
 * Blazor:    Called from OnAfterRenderAsync via IJSRuntime
 */
function initializePage(message) {
    const el = document.getElementById('startup-output');
    if (el) {
        el.textContent = message;
        el.classList.add('text-success');
    }
}

/**
 * Web Forms: ScriptManager.SetFocus("txtSearch")
 * Blazor:    ElementReference + FocusAsync(), or IJSRuntime for advanced cases
 */
function focusElement(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.focus();
        el.classList.add('border-primary');
        return true;
    }
    return false;
}

/**
 * Web Forms: Page.ClientScript.RegisterClientScriptBlock(...) with DOM manipulation
 * Blazor:    Call named JS functions from OnAfterRenderAsync
 */
function highlightElement(elementId, color) {
    const el = document.getElementById(elementId);
    if (el) {
        el.style.backgroundColor = color;
        el.style.transition = 'background-color 0.3s ease';
    }
}
