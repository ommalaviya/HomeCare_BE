using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Public.Api.HomeCare.Extensions;
using Public.Application.HomeCare.Profiles;
using Shared.HomeCare.Middleware;
using System.Text;
using Microsoft.OpenApi;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddControllers();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.RegisterValidators();
builder.Services.RegisterNominatimClient(builder.Configuration);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// DB — reads from Vercel env variable first, falls back to appsettings
var publicConnStr = Environment.GetEnvironmentVariable("PUBLIC_DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HomeCareDbContext>(options =>
    options.UseNpgsql(publicConnStr));

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
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
                Encoding.UTF8.GetBytes(jwt["Key"]
                    ?? throw new InvalidOperationException(string.Format(Messages.NotConfigured, Messages.JwtKey)))),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS — reads frontend URL from Vercel env variable
var publicFrontendUrl = Environment.GetEnvironmentVariable("PUBLIC_FRONTEND_URL")
    ?? builder.Configuration["Frontend:BaseUrl"]
    ?? "http://localhost:4300";

builder.Services.AddCors(o => o.AddPolicy("AllowAngularCustomer", p =>
    p.WithOrigins(
        publicFrontendUrl, 
        "http://localhost:4300",
        "https://homecare-customer-frontend.vercel.app"
    )
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngularCustomer");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();