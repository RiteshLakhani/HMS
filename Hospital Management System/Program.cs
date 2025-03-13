using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add MVC Services
builder.Services.AddControllersWithViews();

// 🔹 Register HttpClient to call the API
builder.Services.AddHttpClient();

// 🔹 Enable Session Support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); // Required for accessing HttpContext in views

var app = builder.Build();

// 🔹 Middleware Configuration
app.UseStaticFiles();
app.UseRouting();

app.UseSession();  // Ensure session middleware is before authentication

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");
});

app.Run();

