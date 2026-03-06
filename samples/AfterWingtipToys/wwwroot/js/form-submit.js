// Bypass Blazor's enhanced form handling for forms with data-enhance="false".
// Must be loaded BEFORE blazor.web.js so our capturing listener runs first.
// Intercept CLICK on submit buttons (Blazor may prevent the submit event from ever firing).
document.addEventListener('click', function (e) {
    var btn = e.target.closest('button[type="submit"], input[type="submit"]');
    if (!btn) return;
    var form = btn.closest('form');
    if (form && form.getAttribute('data-enhance') === 'false') {
        e.stopImmediatePropagation();
        e.preventDefault();
        HTMLFormElement.prototype.submit.call(form);
    }
}, true);
