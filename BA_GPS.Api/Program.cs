using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using BA_GPS.Common;
using BA_GPS.Common.Authentication;
using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure;
using BA_GPS.Infrastructure.Repositories;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Tạo bản ghi mới -> ghi vào bản ghi có địa chỉ ở bên cấu hình appsetting
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Đăng ký cấu hình
ConfigurationManager configuration = builder.Configuration;

// Thêm tính năng uỷ quyền
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Thêm distributed cache (Redis cache) 
builder.Services.AddDistributedMemoryCache();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

    //Thêm bảo mật được định nghĩa là 'Bearer'
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        //Truyền thông tin xác thực qua header
        In = ParameterLocation.Header,

        Description = "Please enter a valid token",
        Name = "Authorization",

        //Loại của token
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
    var authConfig = configuration.GetSection("Auth0");
    // Lấy key
    var secretKey = authConfig["SecretKey"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = authConfig["Issuer"],
        ValidAudience = authConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

//Tự bóp băng thông 
//builder.Services.AddRateLimiter(options =>
//{
//    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

//    options.AddFixedWindowLimiter("fixed", options =>
//    {
//        // Số lượng yêu cầu được phép trong một khoảng thời gian cố định.
//        options.PermitLimit = 10;
//        options.Window = TimeSpan.FromSeconds(10);
//        // Thứ tự xử lý hàng chờ
//        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        // Giới hạn của hàng đợi, số lượng yêu cầu tối đa được chờ đợi trong hàng đợi trước khi bắt đầu từ chối yêu cầu mới.
//        options.QueueLimit = 5;
//    });

//    options.AddSlidingWindowLimiter("sliding", options =>
//    {
//        options.PermitLimit = 10;
//        options.Window = TimeSpan.FromSeconds(10);
//        options.SegmentsPerWindow = 2;
//        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        options.QueueLimit = 5;
//    });
//});

// Thêm database vào service
builder.Services.AddDbContext<GenericDbContext>(otp => otp.UseSqlServer(configuration.GetConnectionString("BAconnection"), b => b.MigrationsAssembly("BA_GPS.Api")));
builder.Services.AddScoped<GenericRepository<User>>();
builder.Services.AddScoped<GenericRepository<UserPermission>>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtUltis>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenService>();
builder.Services.AddScoped<CommonService>();
builder.Services.AddMemoryCache();

// Thêm chính sách CORS 
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

// Thêm vào để thực hiện phân quyền
app.UseAuthentication();

// Thêm vào để thực hiện uỷ quyền
app.UseAuthorization();


// Cấu hình cho đường dẫn yêu cầu HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();


// Chạy Cors
app.UseCors("CorsPolicy");

//app.UseRateLimiter();

app.MapControllers();

app.Run();
