using JWTAuth.Core.Interfaces;
using JWTAuth.Core.Services;
using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Manager;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Db.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new ConfigurationOptions()
    {
        EndPoints = { { builder.Configuration["ConnectionStrings:DefaultRedisCache"] } },
        Password = builder.Configuration["RedisPassword"],
        CheckCertificateRevocation = false,
        KeepAlive = 10,
        ConnectTimeout = 5000,
        ConnectRetry = 5,
        SyncTimeout = 10000,
    };
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var jwtSettings = builder.Configuration.GetSection("TokenConfigurations");
var secretKey = jwtSettings["Secret"];

var signingConfigurations = new SigningConfigurations(secretKey);

var tokenConfigurations = builder.Configuration.GetSection("TokenConfigurations").Get<TokenConfigurations>();

builder.Services.AddSingleton(signingConfigurations);
builder.Services.AddSingleton(tokenConfigurations);
builder.Services.AddTransient(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddCors();

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = tokenConfigurations.Issuer,
            ValidAudience = tokenConfigurations.Audience,
            ClockSkew = TimeSpan.Zero
        };
    })
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});


var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Cookies.ContainsKey("AccessToken"))
    {
        var token = context.Request.Cookies["AccessToken"];
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");
});

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.Run();
