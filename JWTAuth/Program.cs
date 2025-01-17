using JWTAuth.Core.Interfaces;
using JWTAuth.Core.Services;
using JWTAuth.Core.Services.Jwt;
using JWTAuth.Core.Services.Jwt.Manager;
using JWTAuth.Core.Services.Jwt.Models;
using JWTAuth.Db.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var signingConfigurations = new SigningConfigurations(Settings.Secret);
var tokenConfigurations = builder.Configuration.GetSection("TokenConfigurations").Get<TokenConfigurations>();

builder.Services.AddSingleton(signingConfigurations);
builder.Services.AddSingleton(tokenConfigurations);
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddTransient(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();


builder.Services.AddEntityFrameworkNpgsql()
                .AddDbContext<DataContext>(
                    options => options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddCors();

var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.Secret)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = tokenConfigurations.Issuer,
        ValidAudience = tokenConfigurations.Audience,
        ClockSkew = TimeSpan.Zero
    };
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
