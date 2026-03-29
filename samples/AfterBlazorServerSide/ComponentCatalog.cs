namespace AfterBlazorServerSide;

/// <summary>
/// Single source of truth for all components in the sample site.
/// Used for navigation, search, and component listing.
/// </summary>
public static class ComponentCatalog
{
    public static IReadOnlyList<ComponentInfo> Components { get; } = new List<ComponentInfo>
    {
        // Utility Controls
        new("Content", "Utility", "/ControlSamples/Content", "Provides content to ContentPlaceHolder regions in master page layouts"),
        new("ContentPlaceHolder", "Utility", "/ControlSamples/ContentPlaceHolder", "Defines replaceable content regions in master page layouts"),
        new("MasterPage", "Utility", "/control-samples/masterpage", "Master page template support for consistent layouts"),
        new("Localize", "Utility", "/ControlSamples/Localize", "Localization and resource string rendering"),
        new("MultiView", "Utility", "/ControlSamples/MultiView", "Container for multiple View controls with switching"),
        new("View", "Utility", "/ControlSamples/View", "Container panel within a MultiView, visible one at a time"),
        new("PlaceHolder", "Utility", "/ControlSamples/PlaceHolder", "Container for dynamically added controls"),
        new("DataBinder", "Utility", "/ControlSamples/DataBinder", "Data binding helper with Eval() expressions",
            Keywords: new[] { "databind", "eval", "expression" }),
        new("ViewState", "Utility", "/ControlSamples/ViewState", "State persistence across postbacks",
            Keywords: new[] { "state", "persist", "postback" }),

        // Editor Controls
        new("AdRotator", "Editor", "/ControlSamples/AdRotator", "Displays rotating advertisement images"),
        new("BulletedList", "Editor", "/ControlSamples/BulletedList", "Displays items as a bulleted or numbered list",
            new[] { "Selection" },
            new[] { "list", "bullet", "numbered" }),
        new("Button", "Editor", "/ControlSamples/Button", "Server-side button control with click events",
            new[] { "Style", "JavaScript", "CausesValidation", "ValidationGroup" },
            new[] { "submit", "click", "postback" }),
        new("CheckBox", "Editor", "/ControlSamples/CheckBox", "Boolean input control with checked state",
            new[] { "Style", "Events" },
            new[] { "boolean", "checked", "toggle" }),
        new("CheckBoxList", "Editor", "/ControlSamples/CheckBoxList", "List of checkboxes for multiple selections",
            Keywords: new[] { "checkbox", "list", "multiple", "select" }),
        new("DropDownList", "Editor", "/ControlSamples/DropDownList", "Single-selection dropdown list control",
            Keywords: new[] { "select", "combo", "list" }),
        new("FileUpload", "Editor", "/ControlSamples/FileUpload", "File upload input control",
            Keywords: new[] { "upload", "file", "input" }),
        new("HiddenField", "Editor", "/ControlSamples/HiddenField", "Hidden input field for storing values"),
        new("HyperLink", "Editor", "/ControlSamples/HyperLink", "Renders an anchor element for navigation",
            Keywords: new[] { "link", "anchor", "navigation" }),
        new("Image", "Editor", "/ControlSamples/Image", "Displays an image with alt text support",
            Keywords: new[] { "img", "picture" }),
        new("ImageButton", "Editor", "/ControlSamples/ImageButton", "Clickable image that functions as a button",
            Keywords: new[] { "image", "button", "click" }),
        new("ImageMap", "Editor", "/ControlSamples/ImageMap", "Image with clickable hotspot regions"),
        new("Label", "Editor", "/ControlSamples/Label", "Renders text as span or accessible label element",
            Keywords: new[] { "text", "label", "accessibility" }),
        new("LinkButton", "Editor", "/ControlSamples/LinkButton", "Button rendered as a hyperlink",
            new[] { "JavaScript" },
            new[] { "link", "postback" }),
        new("ListBox", "Editor", "/ControlSamples/ListBox", "Scrollable list for single or multiple selection",
            Keywords: new[] { "list", "select", "multi", "scroll" }),
        new("Literal", "Editor", "/ControlSamples/Literal", "Renders text or HTML without additional markup"),
        new("Panel", "Editor", "/ControlSamples/Panel", "Container that renders as a div element",
            new[] { "BackImageUrl" },
            new[] { "container", "div", "group" }),
        new("RadioButton", "Editor", "/ControlSamples/RadioButton", "Single radio button control for mutually exclusive selection",
            Keywords: new[] { "radio", "option", "single" }),
        new("RadioButtonList", "Editor", "/ControlSamples/RadioButtonList", "List of mutually exclusive radio buttons",
            Keywords: new[] { "radio", "options", "select" }),
        new("Table", "Editor", "/ControlSamples/Table", "Programmatically created HTML table",
            Keywords: new[] { "grid", "rows", "cells" }),

        // Data Controls
        new("Calendar", "Data", "/ControlSamples/Calendar", "Interactive calendar for date selection",
            Keywords: new[] { "date", "picker", "month" }),
        new("Chart", "Data", "/ControlSamples/Chart", "Data visualization with multiple chart types",
            new[] { "Area", "Bar", "ChartAreas", "DataBinding", "Doughnut", "Line", "MultiSeries", "Pie", "Scatter", "StackedColumn", "Styling" },
            new[] { "graph", "visualization", "series" }),
        new("DataGrid", "Data", "/ControlSamples/DataGrid", "Legacy grid control with auto-generated columns",
            new[] { "AutoGeneratedColumns", "ShowHideHeader", "Styles" },
            new[] { "grid", "table", "legacy" }),
        new("DataList", "Data", "/ControlSamples/DataList", "Data-bound list with repeating templates",
            new[] { "RepeatColumns", "Flow", "ComplexStyle", "HeaderStyle", "FooterStyle" },
            new[] { "list", "template", "repeat" }),
        new("DataPager", "Data", "/ControlSamples/DataPager", "Paging control for data-bound list controls",
            Keywords: new[] { "pager", "pagination", "page" }),
        new("DetailsView", "Data", "/ControlSamples/DetailsView", "Single-record data display with auto-generated rows",
            new[] { "Caption", "Styles" },
            new[] { "detail", "single", "record" }),
        new("FormView", "Data", "/ControlSamples/FormView/Simple", "Single-record data display and editing",
            new[] { "Edit", "Events", "Styles" },
            new[] { "form", "edit", "single" }),
        new("GridView", "Data", "/ControlSamples/GridView", "Feature-rich data grid with sorting and paging",
            new[] { "AutoGeneratedColumns", "BindAttribute", "DisplayProperties", "InlineEditing", "Paging", "RowSelection", "Selection", "Sorting", "TemplateFields" },
            new[] { "grid", "table", "sort", "page" }),
        new("ListView", "Data", "/ControlSamples/ListView", "Flexible data-bound list with full template control",
            new[] { "CrudOperations", "Grouping", "ItemDataBound", "LayoutTest", "ModelBinding" },
            new[] { "list", "template", "flexible" }),
        new("Repeater", "Data", "/ControlSamples/Repeater", "Basic data-bound repeating template control",
            Keywords: new[] { "repeat", "template", "iterate" }),

        // Validation Controls
        new("CompareValidator", "Validation", "/ControlSamples/CompareValidator", "Validates by comparing to another control or value",
            Keywords: new[] { "compare", "equal", "validation" }),
        new("CustomValidator", "Validation", "/ControlSamples/CustomValidator", "Custom validation logic with server and client functions",
            Keywords: new[] { "custom", "function", "validation" }),
        new("RangeValidator", "Validation", "/ControlSamples/RangeValidator", "Validates value falls within a range",
            Keywords: new[] { "range", "min", "max", "validation" }),
        new("RegularExpressionValidator", "Validation", "/ControlSamples/RegularExpressionValidator", "Validates value matches a regex pattern",
            Keywords: new[] { "regex", "pattern", "validation" }),
        new("RequiredFieldValidator", "Validation", "/ControlSamples/RequiredFieldValidator", "Ensures a field has a value",
            Keywords: new[] { "required", "mandatory", "validation" }),
        new("ModelErrorMessage", "Validation", "/ControlSamples/ModelErrorMessage", "Displays model state errors for a specific key",
            Keywords: new[] { "model", "error", "validation", "key" }),
        new("ValidationSummary", "Validation", "/ControlSamples/ValidationSummary", "Displays summary of all validation errors",
            Keywords: new[] { "summary", "errors", "validation" }),

        // Navigation Controls
        new("Menu", "Navigation", "/ControlSamples/Menu/Selection", "Hierarchical menu navigation control",
            Keywords: new[] { "menu", "navigation", "horizontal", "vertical" }),
        new("SiteMapPath", "Navigation", "/ControlSamples/SiteMapPath", "Breadcrumb navigation based on site map",
            new[] { "Events" },
            new[] { "breadcrumb", "path", "sitemap" }),
        new("TreeView", "Navigation", "/ControlSamples/TreeView", "Hierarchical tree navigation control",
            new[] { "Accessibility", "ArrowsImages", "BulletImages", "BulletsNoExpand", "ExpandCollapse", "ImageAndConfig", "Images", "Selection", "ShowLines", "SiteMapDataSource", "XmlDataSource" },
            new[] { "tree", "hierarchy", "expand", "collapse" }),

        // Login Controls
        new("ChangePassword", "Login", "/ControlSamples/ChangePassword", "Form for changing user password",
            Keywords: new[] { "password", "change", "security" }),
        new("CreateUserWizard", "Login", "/ControlSamples/CreateUserWizard", "Multi-step user registration wizard",
            Keywords: new[] { "register", "signup", "wizard" }),
        new("Login", "Login", "/ControlSamples/Login", "User authentication login form",
            Keywords: new[] { "authenticate", "signin", "credentials" }),
        new("LoginName", "Login", "/ControlSamples/LoginName", "Displays the current user's name",
            Keywords: new[] { "user", "name", "display" }),
        new("LoginStatus", "Login", "/ControlSamples/LoginStatusAuthenticated", "Shows login/logout link based on auth state",
            Keywords: new[] { "status", "logout", "link" }),
        new("LoginView", "Login", "/ControlSamples/LoginView", "Template-based display for authenticated and anonymous users",
            Keywords: new[] { "auth", "template", "anonymous", "authenticated" }),
        new("PasswordRecovery", "Login", "/ControlSamples/PasswordRecovery", "Multi-step password recovery form",
            Keywords: new[] { "password", "recovery", "reset", "security" }),

        // AJAX Controls
        new("CollapsiblePanelExtender", "AJAX", "/ControlSamples/CollapsiblePanelExtender", "Adds collapsible expand/collapse behavior to a target panel",
            Keywords: new[] { "collapse", "expand", "panel", "toggle", "extender", "toolkit" }),
        new("ConfirmButtonExtender", "AJAX", "/ControlSamples/ConfirmButtonExtender", "Attaches a confirmation dialog to a target button",
            Keywords: new[] { "confirm", "dialog", "button", "extender", "toolkit" }),
        new("FilteredTextBoxExtender", "AJAX", "/ControlSamples/FilteredTextBoxExtender", "Restricts text box input to specified character sets",
            Keywords: new[] { "filter", "textbox", "input", "restrict", "extender", "toolkit" }),
        new("ModalPopupExtender", "AJAX", "/ControlSamples/ModalPopupExtender", "Displays a target element as a modal popup with backdrop overlay",
            Keywords: new[] { "modal", "popup", "dialog", "overlay", "extender", "toolkit" }),
        new("Timer", "AJAX", "/ControlSamples/Timer", "Triggers callbacks at timed intervals",
            Keywords: new[] { "timer", "interval", "tick", "polling" }),
        new("UpdatePanel", "AJAX", "/ControlSamples/UpdatePanel", "Partial page update container",
            Keywords: new[] { "partial", "async", "postback", "update" }),
        new("UpdateProgress", "AJAX", "/ControlSamples/UpdateProgress", "Shows loading indicator during async operations",
            Keywords: new[] { "loading", "progress", "indicator", "spinner" }),
        new("Accordion", "AJAX", "/ControlSamples/Accordion", "Container with collapsible panes, only one expanded at a time",
            Keywords: new[] { "accordion", "pane", "collapse", "expand", "container", "toolkit" }),
        new("TabContainer", "AJAX", "/ControlSamples/TabContainer", "Displays content in tabbed panels with tabbed interface",
            Keywords: new[] { "tabs", "tabbed", "panel", "container", "toolkit" }),
        new("CalendarExtender", "AJAX", "/ControlSamples/CalendarExtender", "Attaches a popup calendar date picker to a target textbox",
            Keywords: new[] { "calendar", "date", "picker", "popup", "extender", "toolkit" }),
        new("AutoCompleteExtender", "AJAX", "/ControlSamples/AutoCompleteExtender", "Provides typeahead autocomplete functionality for a target textbox",
            Keywords: new[] { "autocomplete", "typeahead", "suggestions", "dropdown", "extender", "toolkit" }),
        new("MaskedEditExtender", "AJAX", "/ControlSamples/MaskedEditExtender", "Applies input mask to textbox for structured data entry",
            Keywords: new[] { "mask", "input", "formatted", "phone", "date", "extender", "toolkit" }),
        new("NumericUpDownExtender", "AJAX", "/ControlSamples/NumericUpDownExtender", "Adds spinner buttons for numeric value adjustment",
            Keywords: new[] { "numeric", "spinner", "updown", "increment", "extender", "toolkit" }),
        new("SliderExtender", "AJAX", "/ControlSamples/SliderExtender", "Attaches range slider behavior with customizable appearance",
            Keywords: new[] { "slider", "range", "input", "track", "extender", "toolkit" }),
        new("ToggleButtonExtender", "AJAX", "/ControlSamples/ToggleButtonExtender", "Replaces checkbox with clickable image toggle states",
            Keywords: new[] { "toggle", "checkbox", "image", "button", "extender", "toolkit" }),
        new("PopupControlExtender", "AJAX", "/ControlSamples/PopupControlExtender", "Displays popup panel on trigger control click",
            Keywords: new[] { "popup", "panel", "click", "lightweight", "extender", "toolkit" }),
        new("HoverMenuExtender", "AJAX", "/ControlSamples/HoverMenuExtender", "Displays popup menu on hover over target control",
            Keywords: new[] { "hover", "menu", "popup", "mouse", "extender", "toolkit" }),
        new("AjaxToolkitShowcase", "AJAX", "/ControlSamples/AjaxToolkitShowcase", "End-to-end migration showcase demonstrating 10 Ajax Toolkit controls on a single page",
            Keywords: new[] { "showcase", "migration", "demo", "accordion", "tabs", "modal", "autocomplete", "calendar", "mask", "slider", "hover", "toolkit" }),

        // Migration Helpers
        new("ConfigurationManager", "Migration Helpers", "/ControlSamples/Migration/ConfigurationManager", "Static shim for reading AppSettings and ConnectionStrings from IConfiguration",
            Keywords: new[] { "config", "appsettings", "connectionstrings", "web.config", "migration", "shim" }),
        new("NamingContainer", "Migration Helpers", "/ControlSamples/NamingContainer", "Establishes naming scope for child components, equivalent to INamingContainer",
            Keywords: new[] { "naming", "scope", "id", "prefix" }),
        new("ScriptManager", "Migration Helpers", "/ControlSamples/ScriptManager", "Migration stub for AJAX script management",
            Keywords: new[] { "ajax", "script", "migration", "stub" }),
        new("ScriptManagerProxy", "Migration Helpers", "/ControlSamples/ScriptManagerProxy", "Migration stub for content page script references",
            Keywords: new[] { "ajax", "script", "proxy", "migration" }),
        new("Substitution", "Migration Helpers", "/ControlSamples/Substitution", "Post-cache dynamic content substitution",
            Keywords: new[] { "cache", "dynamic", "callback", "substitution" }),

        // Cross-Cutting / Base Properties
        new("BaseProperties", "Utility", "/ControlSamples/BaseProperties", "AccessKey, ToolTip, BackColor, ForeColor and other base class properties",
            Keywords: new[] { "accesskey", "tooltip", "backcolor", "forecolor", "style", "base" }),

        // Theming
        new("Theming", "Theming", "/ControlSamples/Theming", "Skins and Themes PoC with ThemeProvider and ControlSkin",
            Keywords: new[] { "skin", "theme", "provider", "skinid" }),

    }.AsReadOnly();

    public static IReadOnlyList<string> Categories { get; } = Components
        .Select(c => c.Category)
        .Distinct()
        .OrderBy(c => c)
        .ToList()
        .AsReadOnly();

    public static IEnumerable<ComponentInfo> GetByCategory(string category) =>
        Components.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Name);

    public static ComponentInfo? GetByRoute(string route) =>
        Components.FirstOrDefault(c => c.Route.Equals(route, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<ComponentInfo> Search(string query)
    {
        var terms = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return Components.Where(c =>
            terms.All(term =>
                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (c.Keywords?.Any(k => k.Contains(term, StringComparison.OrdinalIgnoreCase)) ?? false)
            )
        );
    }
}

public record ComponentInfo(
    string Name,
    string Category,
    string Route,
    string Description,
    string[]? SubPages = null,
    string[]? Keywords = null
);
