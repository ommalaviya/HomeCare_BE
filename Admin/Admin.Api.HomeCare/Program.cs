
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

// Add services to the container.
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
// Extension Configuration
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

// Register validators
builder.Services.RegisterValidators();

// Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// DbContext configuration
builder.Services.AddDbContext<HomeCareDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR
builder.Services.RegisterSharedSignalRServices();

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
            var accessToken = ctx.Request.Query["access_token"];
            var path = ctx.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                ctx.Token = accessToken;

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

// CORS 
builder.Services.AddCors(o => o.AddPolicy("AllowAngularAdmin", p =>
    p.WithOrigins(
        builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:4200","http://localhost:4300")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//don't alter this order — authentication must come before authorization, and CORS must come before both --pankil
app.UseRouting();
app.UseCors("AllowAngularAdmin");
app.UseImageStaticFiles("Services");
app.UseImageStaticFiles("ServicePartner");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBookingHub();
app.Run();