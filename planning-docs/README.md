# Planning Documentation

Architecture analysis, milestone plans, and component research for BlazorWebFormsComponents.

## Structure

- **[components/](components/)** — Per-component analysis and HTML output research (~58 docs)
- **[milestones/](milestones/)** — Milestone plans, audits, and post-fix reports
- **[analysis/](analysis/)** — Cross-cutting analysis: data controls, login/identity, theming, migration toolkit design
- **[reports/](reports/)** — Executive reports and migration benchmarks
- **[screenshots/](screenshots/)** — Visual references

## Component Docs

Each `components/{ControlName}.md` follows a standard template comparing the original .NET Framework 4.8 API surface against our Blazor component implementation — covering properties, events, methods, and HTML output.

### Status Categories

| Status | Meaning |
|--------|---------|
| ✅ Match | Feature exists in Blazor and works the same as Web Forms |
| ⚠️ Needs Work | Feature exists but is incomplete, buggy, or behaves differently |
| 🔴 Missing | Feature does not exist in the Blazor component |
| N/A | Feature is server-side only and doesn't apply to Blazor (ViewState, PostBack, etc.) |

### Controls Covered

- **Editor Controls (28):** AdRotator, BulletedList, Button, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TextBox, View, Xml
- **Data Controls (9):** Chart, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater
- **Validation Controls (6):** CompareValidator, CustomValidator, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary
- **Navigation Controls (3):** Menu, SiteMapPath, TreeView
- **Login Controls (7):** ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery
