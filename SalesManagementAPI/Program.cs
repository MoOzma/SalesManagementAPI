using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using SalesManagementAPI.Data.Repositories;
using SalesManagementAPI.Helpers;
using SalesManagementAPI.Middleware;
using SalesManagementAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// ── 1. قاعدة البيانات ────────────────────────────────────────────
// تسجيل الـ AppDbContext مع Connection String
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

// ── 2. Repositories ──────────────────────────────────────────────
// تسجيل الـ Generic Repository ليطابق IRepository<Product> و IRepository<User> وغيرهم
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// ── 3. Services ──────────────────────────────────────────────────
// Scoped: نسخة جديدة لكل طلب HTTP Request
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Singleton: نسخة واحدة طوال عمر التطبيق (لا يحتفظ ببيانات مستخدم)
builder.Services.AddSingleton<JwtHelper>();

// ── 4. JWT Authentication ────────────────────────────────────────
// نضبط طريقة التحقق من التوكنات (Tokens) الواردة
var jwtSection = cfg.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            // تحقق أن التوكن صدر من تطبيقنا
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            // تحقق أن التوكن مخصص لتطبيقنا
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            // تحقق أن التوكن لم ينتهِ
            ValidateLifetime = true,

            // تحقق من التوقيع لمنع التوكنات المزورة
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),

            // سماحية 5 دقائق لفارق الوقت بين السيرفر والعميل
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

builder.Services.AddAuthorization();

// ── 5. Swagger مع دعم JWT ────────────────────────────────────────
// إضافة زر Authorize في Swagger لإرسال التوكن مع الطلبات
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "أدخل: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }, Array.Empty<string>()
    }});
});

builder.Services.AddControllers();

var app = builder.Build();

// ── Middleware Pipeline — الترتيب مهم جداً ──────────────────────
// كل Middleware يُنفذ بهذا الترتيب لكل طلب HTTP

// أول شيء — يلتقط أخطاء كل ما يليه
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// يقرأ الـ JWT Token من الـ Authorization Header ويتحقق منه
app.UseAuthentication();

// يجب أن يكون بعد الـ Authentication — يتحقق من الصلاحيات
app.UseAuthorization();

app.MapControllers();

// ── تطبيق Migrations تلقائياً عند البدء ────────────────────────
// مناسب للتطوير — في الـ Production يُفضل تطبيقها يدوياً
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();