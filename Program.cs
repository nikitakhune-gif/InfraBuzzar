using InfraBuzzar.DAL.Interfaces;
using InfraBuzzar.DAL.Repository;
using InfraBuzzar.Data;
using InfraBuzzar.SAL.Interfaces;
using InfraBuzzar.SAL.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use builder.Configuration instead of building a temporary provider
var config = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(item => item.UseSqlServer(config.GetConnectionString("Conn")));

// Register Services (DI)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDistributedMemoryCache();  // Required
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Show detailed errors in development
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for
    // production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session should be enabled before authorization and endpoint mapping
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Wrap Run in try/catch to log unhandled exceptions at startup
try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogCritical(ex, "Host terminated unexpectedly.");
    throw;
}