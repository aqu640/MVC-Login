using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersApp.Data;
using UsersApp.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 創建共享目錄
var keyDirectory = new DirectoryInfo(@"C:\shared-keys-directory");
if (!keyDirectory.Exists)
{
    keyDirectory.Create();
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keyDirectory)
    .SetApplicationName("SharedCookieApp");

// Set Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MyAppAuth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24); // Cookie 有效期
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    // 設定 Cookie 路徑為根目錄
    options.Cookie.Path = "/";

    if (!keyDirectory.Exists)
    {
        keyDirectory.Create();
    }

    // 設定資料保護提供者
    options.DataProtectionProvider = DataProtectionProvider.Create(keyDirectory);
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
