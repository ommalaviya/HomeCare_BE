using Admin.Api.HomeCare.Extensions;
using Admin.Application.HomeCare.Profiles;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Shared.HomeCare.Middleware;
using Shared.HomeCare.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HomeCare Admin API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>() }
    });
});

builder.Services.AddControllers();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.RegisterValidators();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// DB — reads from Vercel env variable first, falls back to appsettings
var adminConnStr = Environment.GetEnvironmentVariable("ADMIN_DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HomeCareDbContext>(options =>
    options.UseNpgsql(adminConnStr));

var jwt = builder.Configuration.GetSection("JwtSettings");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var cookie = ctx.Request.Cookies["hc_token"];
                if (!string.IsNullOrEmpty(cookie))
                    ctx.Token = cookie;
                return Task.CompletedTask;
            }
        };
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwt["Issuer"],
            ValidAudience            = jwt["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["SecretKey"]!)),
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(o =>
    o.AddPolicy("SuperAdminOnly",
        p => p.RequireClaim("isSuperAdmin", "true")));

// CORS — reads frontend URL from Vercel env variable
var adminFrontendUrl = Environment.GetEnvironmentVariable("ADMIN_FRONTEND_URL")
    ?? builder.Configuration["Frontend:BaseUrl"]
    ?? "http://localhost:4200";

builder.Services.AddCors(o => o.AddPolicy("AllowAngularAdmin", p =>
    p.WithOrigins(
        adminFrontendUrl, 
        "http://localhost:4200", 
        "http://localhost:4300",
        "https://homecare-admin-frontend.vercel.app",
        "https://homecare-admin-frontend-2adsz2ajj-oms-projects-de56b349.vercel.app"
    )
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAngularAdmin");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();