using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SalesManagementAPI.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        // يُحقن تلقائياً ويُمكّننا من قراءة appsettings.json
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            // الخطوة 1: تحويل السر النصي لمفتاح تشفير — التشفير يعمل على bytes وليس نصاً
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // الخطوة 2: تحديد خوارزمية التوقيع — HmacSha256 آمن وسريع
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // الخطوة 3: بناء الـ Claims (بيانات المستخدم المشفرة داخل التوكن)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // المعرّف القياسي
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role), // يستخدمه [Authorize(Roles="Admin")] تلقائياً
                new Claim("userId", user.Id.ToString()) // Claim مخصص لسهولة الوصول في الكود
            };

            // الخطوة 4: إنشاء التوكن وضبط مدة الصلاحية
            var expiryHours = double.Parse(_config["Jwt:ExpiryInHours"]!);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours), // UtcNow دائماً لتوحيد التوقيت عالمياً
                signingCredentials: credentials
            );

            // الخطوة 5: تحويل التوكن لنص (String) لإرساله للـ Client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }///KL;M;
    }
}