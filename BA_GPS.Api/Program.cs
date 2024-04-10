using System.Text;
using System.Text.Json.Serialization;
using BA_GPS.Common.Authentication;
using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure;
using BA_GPS.Infrastructure.Repositories;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Register configuration
ConfigurationManager configuration = builder.Configuration;

//builder.Services.AddControllers().AddJsonOptions(x =>
//{

//    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//    x.JsonSerializerOptions.AllowTrailingCommas = true;
//});

// Add authorization
builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "localhost:5159, localhost:4200",
        ValidAudience = "localhost:5159, localhost:4200",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("gUiGDL2oyCn2zR1fVOmJNEUDChihatgi"))
    };
});

// Add database service
builder.Services.AddDbContext<GenericDbContext>(otp => otp.UseSqlServer(configuration.GetConnectionString("BAconnection"), b => b.MigrationsAssembly("BA_GPS.Api")));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<GenericRepository<User>>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtUltis>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenService>();

builder.Services.AddMemoryCache();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Add this line to enable authentication
app.UseAuthentication();

// Add this line to enable authorization
app.UseAuthorization();

// Add this line to enable CORS
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
