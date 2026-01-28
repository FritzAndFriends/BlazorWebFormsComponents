// ES Module version of Basepage.js
// This can be imported automatically by components

export function setTitle(title) {
    document.title = title;
}

export function getTitle() {
    return document.title;
}

export function onAfterRender() {
    console.debug("Running Window.load function");
    formatClientClick();
}

function formatClientClick() {
    var elementsToReplace = document.querySelectorAll("*[onclientclick]");
    for (var el of elementsToReplace) {
        if (!el.getAttribute("data-onclientclick")) {
            console.debug(el.getAttribute("onclientclick"));
            el.addEventListener('click', function (e) { eval(e.target.getAttribute('onclientclick')) });
            el.setAttribute("data-onclientclick", "1");
        }
    }
}

// Also expose on window for backward compatibility
if (typeof window !== 'undefined') {
    window.bwfc = window.bwfc ?? {};
    window.bwfc.Page = {
        setTitle,
        getTitle,
        OnAfterRender: onAfterRender
    };
}
