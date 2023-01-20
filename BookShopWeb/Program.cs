using BookShop.DataAccess;
using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookShop.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add All Controllers with views in the Program.cs
builder.Services.AddControllersWithViews();

//Add Setup of DbContext with SQL SERVER
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
//Map Stripe Account in appsettings with class utility
builder.Services.Configure<StripeS>(builder.Configuration.GetSection("StripeSettings"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Register DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Singleton implementation on Email
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// For Using wwwroot and static Files 
app.UseStaticFiles();

app.UseRouting();

//Add Stripe at global level
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings:SecretKey").Get<String>();

app.UseAuthentication();

app.UseAuthorization();

// To Route Razor Pages
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
