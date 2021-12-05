using Empty_MVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;

//Get Config
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

//Int App Builer
var builder = WebApplication.CreateBuilder(args);

//Add Auth Serives
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            //Cookie Auth Settings
            options.Cookie.Name = "Empy_MVC";
            options.LoginPath = "/Users/Login";
        });

//Session
builder.Services.AddDistributedMemoryCache();

//Add Session w/options
builder.Services.AddSession(options => {
    options.Cookie.Name = ".EMpty.Session";
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add Dependecy Injection
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

//Connection String
builder.Services.AddDbContextPool<MyDataContext>(options => options.UseMySql(config.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection"))));

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
