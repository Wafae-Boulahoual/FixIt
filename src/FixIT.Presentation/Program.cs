using FixIT.Domain.Models;
using FixIT.Infrastracture;
using FixIT.Infrastracture.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false) // "false" Slutar kräva bekräftad e-post för att logga in.
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FixITDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("FixITApi", c => c.BaseAddress = new Uri("https://localhost:7113/"));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    foreach (var role in new[] { "Admin", "Kund", "Tekniker" })
    {
        if (!await roleManager.RoleExistsAsync(role)) 
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@fixit.se";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = "Admin",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin111!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
// blockerar åtkomst till registeringssidan för användare.(bara admin kan göra det)
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
//    {
//        context.Response.StatusCode = 404;
//        return;
//    }
//    await next();
//});
app.MapRazorPages()
   .WithStaticAssets();

app.Run();


// Use This line to update your database....
// Update-Database -Project FixIT.Infrastracture -StartupProject FixIT.Presentation
