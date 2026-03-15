// CalendarExtender JS behavior module
// Attaches a popup calendar date picker to a target input element.

const behaviors = new Map();

// CalendarPosition (matches C# enum)
const POS_BOTTOM_LEFT = 0;
const POS_BOTTOM_RIGHT = 1;
const POS_TOP_LEFT = 2;
const POS_TOP_RIGHT = 3;
const POS_RIGHT = 4;
const POS_LEFT = 5;

// CalendarDefaultView (matches C# enum)
const VIEW_DAYS = 0;
const VIEW_MONTHS = 1;
const VIEW_YEARS = 2;

const MONTH_NAMES = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];
const DAY_HEADERS = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];

/**
 * Creates a calendar behavior and attaches it to the target element.
 * @param {{ targetId: string, behaviorId: string, properties: object }} config
 * @returns {object} A JS object reference for the behavior instance.
 */
export function createBehavior(config) {
    const { targetId, behaviorId, properties } = config;
    const target = document.getElementById(targetId);

    if (!target) {
        console.warn(`[CalendarExtender] Target element '${targetId}' not found.`);
        return {};
    }

    const today = properties.todaysDate ? new Date(properties.todaysDate) : new Date();
    const selected = properties.selectedDate ? new Date(properties.selectedDate) : null;

    const state = {
        targetId,
        properties: { ...properties },
        viewDate: selected ? new Date(selected) : new Date(today),
        currentView: properties.defaultView || VIEW_DAYS,
        popup: null,
        handlers: {},
        isOpen: false
    };

    // Create popup container
    const popup = document.createElement("div");
    popup.style.position = "absolute";
    popup.style.zIndex = "10000";
    popup.style.display = "none";
    popup.style.backgroundColor = "#fff";
    popup.style.border = "1px solid #ccc";
    popup.style.boxShadow = "0 2px 8px rgba(0,0,0,0.15)";
    popup.style.padding = "8px";
    popup.style.minWidth = "220px";
    if (properties.cssClass) {
        popup.className = properties.cssClass;
    }
    document.body.appendChild(popup);
    state.popup = popup;

    function parseDate(str) {
        if (!str) return null;
        const d = new Date(str);
        return isNaN(d.getTime()) ? null : d;
    }

    function formatDate(date) {
        const fmt = properties.format || "d";
        const mm = String(date.getMonth() + 1).padStart(2, "0");
        const dd = String(date.getDate()).padStart(2, "0");
        const yyyy = date.getFullYear();
        if (fmt === "d" || fmt === "MM/dd/yyyy") return `${mm}/${dd}/${yyyy}`;
        if (fmt === "yyyy-MM-dd") return `${yyyy}-${mm}-${dd}`;
        if (fmt === "dd/MM/yyyy") return `${dd}/${mm}/${yyyy}`;
        return `${mm}/${dd}/${yyyy}`;
    }

    function isDateInRange(date) {
        const start = parseDate(properties.startDate);
        const end = parseDate(properties.endDate);
        if (start && date < start) return false;
        if (end && date > end) return false;
        return true;
    }

    function isSameDay(a, b) {
        return a && b &&
            a.getFullYear() === b.getFullYear() &&
            a.getMonth() === b.getMonth() &&
            a.getDate() === b.getDate();
    }

    function selectDate(date) {
        if (!isDateInRange(date)) return;
        target.value = formatDate(date);
        target.dispatchEvent(new Event("input", { bubbles: true }));
        target.dispatchEvent(new Event("change", { bubbles: true }));
        if (properties.onClientDateSelectionChanged) {
            try { new Function(properties.onClientDateSelectionChanged)(); } catch (_) { }
        }
        closePopup();
    }

    function renderDaysView() {
        const year = state.viewDate.getFullYear();
        const month = state.viewDate.getMonth();
        const firstDay = new Date(year, month, 1).getDay();
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const selectedDate = parseDate(target.value) || selected;

        let html = `<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:4px;">`;
        html += `<button type="button" data-action="prev-month" style="border:none;background:none;cursor:pointer;font-size:14px;">&lt;</button>`;
        html += `<span data-action="show-months" style="cursor:pointer;font-weight:bold;">${MONTH_NAMES[month]} ${year}</span>`;
        html += `<button type="button" data-action="next-month" style="border:none;background:none;cursor:pointer;font-size:14px;">&gt;</button>`;
        html += `</div>`;
        html += `<table style="border-collapse:collapse;width:100%;text-align:center;"><thead><tr>`;
        for (const dh of DAY_HEADERS) {
            html += `<th style="padding:2px 4px;font-size:12px;">${dh}</th>`;
        }
        html += `</tr></thead><tbody><tr>`;
        for (let i = 0; i < firstDay; i++) html += `<td></td>`;
        for (let d = 1; d <= daysInMonth; d++) {
            const date = new Date(year, month, d);
            const inRange = isDateInRange(date);
            const isToday = isSameDay(date, today);
            const isSel = isSameDay(date, selectedDate);
            let style = "padding:4px;cursor:" + (inRange ? "pointer" : "default") + ";font-size:13px;";
            if (!inRange) style += "color:#ccc;";
            if (isToday) style += "font-weight:bold;";
            if (isSel) style += "background:#0078d4;color:#fff;border-radius:3px;";
            html += `<td style="${style}" data-action="select-day" data-day="${d}">${d}</td>`;
            if ((firstDay + d) % 7 === 0 && d < daysInMonth) html += `</tr><tr>`;
        }
        html += `</tr></tbody></table>`;
        html += `<div style="text-align:center;margin-top:4px;"><button type="button" data-action="today" style="border:none;background:none;cursor:pointer;font-size:12px;color:#0078d4;">Today</button></div>`;
        popup.innerHTML = html;
    }

    function renderMonthsView() {
        const year = state.viewDate.getFullYear();
        let html = `<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:4px;">`;
        html += `<button type="button" data-action="prev-year" style="border:none;background:none;cursor:pointer;font-size:14px;">&lt;</button>`;
        html += `<span data-action="show-years" style="cursor:pointer;font-weight:bold;">${year}</span>`;
        html += `<button type="button" data-action="next-year" style="border:none;background:none;cursor:pointer;font-size:14px;">&gt;</button>`;
        html += `</div>`;
        html += `<table style="border-collapse:collapse;width:100%;text-align:center;"><tbody>`;
        for (let r = 0; r < 4; r++) {
            html += `<tr>`;
            for (let c = 0; c < 3; c++) {
                const m = r * 3 + c;
                html += `<td style="padding:8px;cursor:pointer;font-size:13px;" data-action="select-month" data-month="${m}">${MONTH_NAMES[m].substring(0, 3)}</td>`;
            }
            html += `</tr>`;
        }
        html += `</tbody></table>`;
        popup.innerHTML = html;
    }

    function renderYearsView() {
        const year = state.viewDate.getFullYear();
        const startYear = year - (year % 10);
        let html = `<div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:4px;">`;
        html += `<button type="button" data-action="prev-decade" style="border:none;background:none;cursor:pointer;font-size:14px;">&lt;</button>`;
        html += `<span style="font-weight:bold;">${startYear} - ${startYear + 9}</span>`;
        html += `<button type="button" data-action="next-decade" style="border:none;background:none;cursor:pointer;font-size:14px;">&gt;</button>`;
        html += `</div>`;
        html += `<table style="border-collapse:collapse;width:100%;text-align:center;"><tbody>`;
        for (let r = 0; r < 4; r++) {
            html += `<tr>`;
            for (let c = 0; c < 3; c++) {
                const y = startYear + r * 3 + c;
                if (y > startYear + 9) {
                    html += `<td></td>`;
                } else {
                    html += `<td style="padding:8px;cursor:pointer;font-size:13px;" data-action="select-year" data-year="${y}">${y}</td>`;
                }
            }
            html += `</tr>`;
        }
        html += `</tbody></table>`;
        popup.innerHTML = html;
    }

    function render() {
        switch (state.currentView) {
            case VIEW_MONTHS: renderMonthsView(); break;
            case VIEW_YEARS: renderYearsView(); break;
            default: renderDaysView(); break;
        }
    }

    function positionPopup() {
        const rect = target.getBoundingClientRect();
        const pos = properties.popupPosition || POS_BOTTOM_LEFT;
        let top, left;
        switch (pos) {
            case POS_BOTTOM_RIGHT: top = rect.bottom + window.scrollY; left = rect.right + window.scrollX - popup.offsetWidth; break;
            case POS_TOP_LEFT: top = rect.top + window.scrollY - popup.offsetHeight; left = rect.left + window.scrollX; break;
            case POS_TOP_RIGHT: top = rect.top + window.scrollY - popup.offsetHeight; left = rect.right + window.scrollX - popup.offsetWidth; break;
            case POS_RIGHT: top = rect.top + window.scrollY; left = rect.right + window.scrollX + 4; break;
            case POS_LEFT: top = rect.top + window.scrollY; left = rect.left + window.scrollX - popup.offsetWidth - 4; break;
            default: top = rect.bottom + window.scrollY; left = rect.left + window.scrollX; break;
        }
        popup.style.top = `${top}px`;
        popup.style.left = `${left}px`;
    }

    function openPopup() {
        if (state.isOpen) return;
        state.currentView = properties.defaultView || VIEW_DAYS;
        render();
        popup.style.display = "block";
        positionPopup();
        state.isOpen = true;
    }

    function closePopup() {
        popup.style.display = "none";
        state.isOpen = false;
    }

    // Popup click delegation
    state.handlers.popupClick = function (e) {
        const el = e.target.closest("[data-action]");
        if (!el) return;
        const action = el.dataset.action;
        switch (action) {
            case "prev-month": state.viewDate.setMonth(state.viewDate.getMonth() - 1); render(); break;
            case "next-month": state.viewDate.setMonth(state.viewDate.getMonth() + 1); render(); break;
            case "prev-year": state.viewDate.setFullYear(state.viewDate.getFullYear() - 1); render(); break;
            case "next-year": state.viewDate.setFullYear(state.viewDate.getFullYear() + 1); render(); break;
            case "prev-decade": state.viewDate.setFullYear(state.viewDate.getFullYear() - 10); render(); break;
            case "next-decade": state.viewDate.setFullYear(state.viewDate.getFullYear() + 10); render(); break;
            case "show-months": state.currentView = VIEW_MONTHS; render(); break;
            case "show-years": state.currentView = VIEW_YEARS; render(); break;
            case "select-day": {
                const day = parseInt(el.dataset.day, 10);
                selectDate(new Date(state.viewDate.getFullYear(), state.viewDate.getMonth(), day));
                break;
            }
            case "select-month": {
                const month = parseInt(el.dataset.month, 10);
                state.viewDate.setMonth(month);
                state.currentView = VIEW_DAYS;
                render();
                break;
            }
            case "select-year": {
                const year = parseInt(el.dataset.year, 10);
                state.viewDate.setFullYear(year);
                state.currentView = VIEW_MONTHS;
                render();
                break;
            }
            case "today": selectDate(new Date(today)); break;
        }
    };
    popup.addEventListener("click", state.handlers.popupClick);

    // Open on focus/click
    state.handlers.targetFocus = () => openPopup();
    target.addEventListener("focus", state.handlers.targetFocus);
    target.addEventListener("click", state.handlers.targetFocus);

    // Close on outside click
    state.handlers.documentClick = function (e) {
        if (!state.isOpen) return;
        if (!popup.contains(e.target) && e.target !== target) {
            closePopup();
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

    const target = document.getElementById(state.targetId);
    if (target) {
        target.removeEventListener("focus", state.handlers.targetFocus);
        target.removeEventListener("click", state.handlers.targetFocus);
    }
    document.removeEventListener("mousedown", state.handlers.documentClick);

    if (state.popup) {
        state.popup.removeEventListener("click", state.handlers.popupClick);
        state.popup.remove();
    }

    behaviors.delete(behaviorId);
}
