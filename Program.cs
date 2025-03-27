// 設定 Identity
// https://learn.microsoft.com/zh-tw/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&utm_source=chatgpt.com&tabs=visual-studio
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersApp.Data;

using UsersApp.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using Microsoft.OpenApi.Models;

// JWT
/*
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
*/
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

// Set Cookie Identity
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
/*
// JWT Authentication
// https://learn.microsoft.com/zh-tw/aspnet/core/security/authentication/?view=aspnetcore-8.0&utm_source=chatgpt.com

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
*/

// 添加 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserApp API", Version = "v1" });

    // 添加 Cookie 認證支援
    c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "MyAppAuth",
        Description = "Cookie 身份驗證"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Cookie" }
            },
            new string[] { }
        }
    });
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
app.UseAuthentication(); // 呼叫相依於驗證中使用者的中介軟體
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserApp API v1"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
