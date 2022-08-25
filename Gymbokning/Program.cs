using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MapperProfile));

var app = builder.Build();


// SeedData

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var db = services.GetRequiredService<ApplicationDbContext>();

//    db.Database.EnsureDeleted();
//    db.Database.Migrate();

//    var config = services.GetRequiredService<IConfiguration>();
//    var adminPW = config["AdminPW"];

//    try
//    {
//        SeedData.InitAsync(db, services, adminPW).GetAwaiter().GetResult();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex);
//        throw;
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/GymClasses/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GymClasses}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
