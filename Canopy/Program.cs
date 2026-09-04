using Canopy.Data;
using Canopy.Hubs;
using Canopy.Repositories;
using Canopy.Repositories.TaskManager.Repositories;
using Canopy.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//db setup - uses Postgres on Railway, SQL Server locally
var pgConn = builder.Configuration.GetConnectionString("PostgresConnection");
var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (!string.IsNullOrEmpty(pgConn))
        options.UseNpgsql(pgConn, x => x.MigrationsAssembly("Canopy").MigrationsHistoryTable("__EFMigrationsHistory_pg"));
    else
        options.UseSqlServer(sqlConn);
});
builder.Services.AddDataProtection()
    .SetApplicationName("Canopy")
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\CanopyKeys"));


// Register repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
builder.Services.AddScoped<IChatsRepository, ChatsRepository>();
builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();


builder.Services.AddSignalR();

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


//jwt token
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            context.HandleResponse();

            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                context.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                var loginUrl = "/home/Login?returnUrl=" + Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
                context.Response.Redirect(loginUrl);
            }

            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization();


builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMessageEncryptionService, MessageEncryptionService>();
builder.Services.AddScoped<ITaskReminderService, TaskReminderService>();

builder.Services.AddHostedService<DailyReminderBackgroundService>();


builder.Services.AddSwaggerGen();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("hubs/chat");
app.MapHub<NotificationHub>("hubs/notification");


app.MapControllers();   

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
// Create database schema on startup when using Postgres (Railway)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var pgConnection = builder.Configuration.GetConnectionString("PostgresConnection");
    if (!string.IsNullOrEmpty(pgConnection))
        db.Database.EnsureCreated();
}

app.Run();
