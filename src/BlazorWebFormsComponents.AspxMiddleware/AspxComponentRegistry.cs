namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Maps lowercase asp: tag names to BWFC Blazor component types.
/// </summary>
public static class AspxComponentRegistry
{
    private static readonly Dictionary<string, Type> s_registry = new(StringComparer.OrdinalIgnoreCase)
    {
        // Editor controls
        ["adrotator"] = typeof(AdRotator),
        ["button"] = typeof(Button),
        ["bulletedlist"] = typeof(BulletedList<object>),
        ["calendar"] = typeof(Calendar),
        ["checkbox"] = typeof(CheckBox),
        ["checkboxlist"] = typeof(CheckBoxList<object>),
        ["dropdownlist"] = typeof(DropDownList<object>),
        ["fileupload"] = typeof(FileUpload),
        ["hiddenfield"] = typeof(HiddenField),
        ["hyperlink"] = typeof(HyperLink),
        ["image"] = typeof(Image),
        ["imagebutton"] = typeof(ImageButton),
        ["imagemap"] = typeof(ImageMap),
        ["label"] = typeof(Label),
        ["linkbutton"] = typeof(LinkButton),
        ["listbox"] = typeof(ListBox<object>),
        ["literal"] = typeof(Literal),
        ["localize"] = typeof(Localize),
        ["multiview"] = typeof(MultiView),
        ["panel"] = typeof(Panel),
        ["placeholder"] = typeof(PlaceHolder),
        ["radiobutton"] = typeof(RadioButton),
        ["radiobuttonlist"] = typeof(RadioButtonList<object>),
        ["table"] = typeof(Table),
        ["tablecell"] = typeof(TableCell),
        ["tableheadercell"] = typeof(TableHeaderCell),
        ["tablerow"] = typeof(TableRow),
        ["textbox"] = typeof(TextBox),
        ["view"] = typeof(View),

        // Data controls (use object as type arg for Phase 1 — markup only)
        ["chart"] = typeof(Chart),
        ["datagrid"] = typeof(DataGrid<object>),
        ["datalist"] = typeof(DataList<object>),
        ["datapager"] = typeof(DataPager),
        ["detailsview"] = typeof(DetailsView<object>),
        ["formview"] = typeof(FormView<object>),
        ["gridview"] = typeof(GridView<object>),
        ["listview"] = typeof(ListView<object>),
        ["repeater"] = typeof(Repeater<object>),

        // Validation controls
        ["comparevalidator"] = typeof(Validations.CompareValidator<object>),
        ["customvalidator"] = typeof(Validations.CustomValidator),
        ["rangevalidator"] = typeof(Validations.RangeValidator<object>),
        ["regularexpressionvalidator"] = typeof(Validations.RegularExpressionValidator),
        ["requiredfieldvalidator"] = typeof(Validations.RequiredFieldValidator<object>),

        // Navigation
        ["menu"] = typeof(Menu),
        ["sitemappath"] = typeof(SiteMapPath),
        ["treeview"] = typeof(TreeView),

        // Login controls
        ["changepassword"] = typeof(LoginControls.ChangePassword),
        ["createuserwizard"] = typeof(LoginControls.CreateUserWizard),
        ["login"] = typeof(LoginControls.Login),
        ["loginname"] = typeof(LoginControls.LoginName),
        ["loginstatus"] = typeof(LoginControls.LoginStatus),
        ["loginview"] = typeof(LoginControls.LoginView),
        ["passwordrecovery"] = typeof(LoginControls.PasswordRecovery),

        // Infrastructure / layout
        ["content"] = typeof(Content),
        ["contentplaceholder"] = typeof(ContentPlaceHolder),
        ["masterpage"] = typeof(MasterPage),
        ["namingcontainer"] = typeof(NamingContainer),
        ["scriptmanager"] = typeof(ScriptManager),
        ["scriptmanagerproxy"] = typeof(ScriptManagerProxy),
        ["updatepanel"] = typeof(UpdatePanel),
        ["updateprogress"] = typeof(UpdateProgress),
        ["timer"] = typeof(Timer),
        ["substitution"] = typeof(Substitution),
    };

    /// <summary>
    /// Resolve an asp: tag name to its BWFC component type.
    /// Returns null if the tag is not recognized.
    /// </summary>
    public static Type? Resolve(string aspTagName)
    {
        return s_registry.GetValueOrDefault(aspTagName);
    }

    /// <summary>
    /// Returns the count of registered component mappings.
    /// </summary>
    public static int Count => s_registry.Count;

    /// <summary>
    /// Returns all registered tag names (lowercase).
    /// </summary>
    public static IEnumerable<string> RegisteredTags => s_registry.Keys;
}
