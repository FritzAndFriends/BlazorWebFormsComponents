// bwfc-webformsform.js — FormData interop for WebFormsForm component
// ES module: reads form field values from the DOM and returns them as a dictionary.

/**
 * Reads all form field values from the given form element.
 * Returns an object where keys are field names and values are string arrays,
 * supporting multi-value fields (checkboxes, multi-select).
 * @param {HTMLFormElement} formElement - The form DOM element.
 * @returns {Object.<string, string[]>} Dictionary of field name to values.
 */
export function readFormData(formElement) {
    if (!formElement) {
        console.warn('[BWFC] readFormData: formElement is null');
        return {};
    }

    var formData = new FormData(formElement);
    var result = {};

    formData.forEach(function (value, key) {
        if (!result[key]) {
            result[key] = [];
        }
        result[key].push(value.toString());
    });

    console.debug('[BWFC] readFormData: read', Object.keys(result).length, 'fields');
    return result;
}
