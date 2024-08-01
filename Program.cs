using dotenv.net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelLocationManagement.Data;
using TravelLocationManagement.Models;
using TravelLocationManagement.Utilities;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotEnv.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TravelLocationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Location Management API", Version = "v1" });
});


// Add identity services
builder.Services.AddIdentity<User, Role>(options =>
{
    // -- Password settings --
    // -----------------------

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true; 
    options.Password.RequireUppercase = true; 
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // -- Lockout settings --
    // ----------------------

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
    options.Lockout.MaxFailedAccessAttempts = 6; 
    options.Lockout.AllowedForNewUsers = true;


    // -- User settings --
    // -------------------

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; 

    // -- Sign-in settings --
    // ----------------------

    options.SignIn.RequireConfirmedEmail = true; 
    options.SignIn.RequireConfirmedPhoneNumber = false; 
})
.AddEntityFrameworkStores<TravelLocationContext>()
.AddDefaultTokenProviders();

// Configure authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("GoogleClientId is missing.");
    options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("GoogleClientSecret is missing.");
});

// Configure HTTPS redirection
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 7245; // Ensure this matches your HTTPS port
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Seed roles and admin user into the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        if (app.Environment.IsDevelopment())
        {
            await DevelopmentSeedData.InitializeAsync(services);
        }
        else
        {
            await SeedData.InitializeAsync(services);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

await app.RunAsync();