# BlazorWebFormsComponents
A collection of Blazor components that emulate the web forms components of the same name

## Controls proposed to be migrated
There are a significant number of controls in ASP.NET Web Forms, and we will focus on creating components in the following order:

  - Data Controls
    - Chart(?)
    - DataList
    - DataPager
    - DetailsView
    - FormView
    - GridView
    - [ListView](docs/ListView.md)
    - Repeater
  - Validation Controls
    - CompareValidator
    - CustomValidator
    - RangeValidator
    - RegularExpressionValidator(?)
    - RequiredFieldValidator
    - ValidationSummary
  - Navigation Controls
    - Menu
    - SiteMapPath
    - TreeView
  - Login Controls
    - ChangePassword
    - Login
    - LoginName
    - LoginStatus
    - LoginView
    - PasswordRecovery

We will NOT be converting any DataSource objects, Wizard components, skins or themes.  Once this first collection of 23 controls is written, we can consider additional features like modern tag formatting.