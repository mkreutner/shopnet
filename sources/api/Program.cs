using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;
using ShopNetApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Enregistrement du contexte de base de données (PostgreSQL)
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MainDatabase")));

// 2. Enregistrement d'Identity avec le support des GUID et de nos entités personnalisées
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Vous pourrez configurer les politiques de mot de passe ici plus tard
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<MainDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Vérification des arguments CLI au lancement
if (args.Length > 0 && args[0].StartsWith("--"))
{
    using (var scope = app.Services.CreateScope())
    {
        await ShopNetApi.Commands.UserCliCommand.Execute(scope.ServiceProvider, args);
    }
    return; // Arrête l'application après exécution de la commande
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware de base
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
