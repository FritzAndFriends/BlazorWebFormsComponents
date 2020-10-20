# Target Application Template

This is a base Blazor Server-Side project that can be used as a boiler-plate to migrate ASP.NET Web Forms applications.  Some brief explanations of folders and suggested mappings:

- **Components**: ASCX user controls that are converted to Blazor Components
- **Models**: App_Data, Data, and Models objects used for interacting with the database
- **Pages**: All .ASPX pages can be delivered to this folder
- **Shared**: MasterPages can be dropped here.  The default layout page for this template has been renamed to `Site.razor` to match the default `Site.master` MasterPage name in Web Forms.
- **wwwroot**: Static content like CSS, Fonts, JavaScript, and images can be placed here in the same folder structure they resided in the original application.  With this strategy, references to the original folder hierarchy will continue to work
