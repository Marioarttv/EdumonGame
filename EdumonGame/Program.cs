using EdumonGame.Components;
using EdumonGame.Components.Account;
using EdumonGame.Data;
using EdumonGame.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Register your game services
builder.Services.AddSingleton<IEdumonDbService, EdumonDbService>();
builder.Services.AddSingleton<IMoveDbService, MoveDbService>();
builder.Services.AddSingleton<IConditionDbService, ConditionDbService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // Add detailed errors for development
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });
// Game state management
builder.Services.AddScoped<GameState>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Initialize the static DB helper classes during app startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Get services
    var edumonDbService = services.GetRequiredService<IEdumonDbService>();
    var moveDbService = services.GetRequiredService<IMoveDbService>();
    var conditionDbService = services.GetRequiredService<IConditionDbService>();

    // Initialize services asynchronously but wait for them to complete
    Task.WhenAll(
        edumonDbService.InitializeAsync(),
        moveDbService.InitializeAsync(),
        conditionDbService.InitializeAsync()
    ).GetAwaiter().GetResult();

    // Set static access classes
    EdumonDB.Initialize(edumonDbService);
    MoveDB.Initialize(moveDbService);
    ConditionsDB.Initialize(conditionDbService);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
