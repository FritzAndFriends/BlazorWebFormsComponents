// TODO: Review and adjust this generated Program.cs for your application needs.
using BlazorWebFormsComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

// TODO: Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
// builder.Services.AddDbContextFactory<ProductContext>(options => options.UseSqlite("Data Source=app.db"));

// TODO: Configure Identity
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddEntityFrameworkStores<ProductContext>();

// TODO: Configure session for cart/state management
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

// TODO: Add middleware in the pipeline
// app.UseSession();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapRazorComponents<WingtipToys.Components.App>()
    .AddInteractiveServerRenderMode();


// --- Redirect Handler Pages (convert to minimal API endpoints) ---
// TODO: CheckoutStart was a redirect handler — convert to minimal API endpoint

app.Run();

