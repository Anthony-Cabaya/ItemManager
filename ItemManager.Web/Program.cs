using ItemManager.Core.Interfaces;
using ItemManager.Infrastructure.Helpers;
using ItemManager.Infrastructure.Repositories;
using ItemManager.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbHelper
builder.Services.AddScoped<DbHelper>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    return new DbHelper(connectionString);
});

// Register Repositories (Dependency)
builder.Services.AddScoped<IItemTypeRepository, ItemTypeRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemSubTypeRepository, ItemSubTypeRepository>();
builder.Services.AddScoped<IUnitCategoryRepository, UnitCategoryRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IItemUnitConversionRepository, ItemUnitConversionRepository>();
builder.Services.AddScoped<IItemCodeRepository, ItemCodeRepository>();
builder.Services.AddScoped<IItemCodeService, ItemCodeService>();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
