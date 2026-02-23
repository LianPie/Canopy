using Canopy.Data;
using Canopy.Repositories;
using Canopy.Repositories.TaskManager.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//db setup- reads connection string from appsetting.json - uses ApplicationDbContext  to interact with the db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// Register repository
builder.Services.AddScoped<IUserRepository, UserRepository>();




//so we can use distributed cache(distributed cache -> cache that can be used everywhere in the program) like session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();



//localization service
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("fa") 
};
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(opts =>
    {
        // All validation attributes will look in SharedResources
        opts.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(Canopy.SharedResources));
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new QueryStringRequestCultureProvider(),   
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});
builder.Services.AddMvc()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(); 



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
