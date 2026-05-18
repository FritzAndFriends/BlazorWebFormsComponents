// bwfc-postback.js — PostBack interop for BlazorWebFormsComponents
// Provides __doPostBack and callback bridge functions that mirror Web Forms behavior.
// Loaded automatically by WebFormsPageBase; may also be included via <script> tag.

(function () {
    'use strict';

    window.__bwfc = window.__bwfc || {};
    window.__bwfc.postBackTargets = window.__bwfc.postBackTargets || {};

    // Classic __doPostBack — same signature as Web Forms
    window.__doPostBack = function (eventTarget, eventArgument) {
        var target = window.__bwfc.postBackTargets[eventTarget];
        if (target) {
            target.invokeMethodAsync('HandlePostBackFromJs', eventTarget, eventArgument);
        } else {
            console.warn('[BWFC] No postback handler for:', eventTarget);
        }
    };

    // Register a .NET component as a postback target
    window.__bwfc.registerPostBackTarget = function (id, dotNetRef) {
        window.__bwfc.postBackTargets[id] = dotNetRef;
    };

    // Unregister a postback target (called on dispose)
    window.__bwfc.unregisterPostBackTarget = function (id) {
        delete window.__bwfc.postBackTargets[id];
    };

    // Callback bridge — mirrors Web Forms ICallbackEventHandler pattern
    window.__bwfc_callback = function (id, arg, successCallback, context, errorCallback) {
        var target = window.__bwfc.postBackTargets[id];
        if (target) {
            target.invokeMethodAsync('HandleCallbackFromJs', id, arg)
                .then(function (result) { if (successCallback) successCallback(result, context); })
                .catch(function (err) { if (errorCallback) errorCallback(err.message, context); });
        }
    };

})();
