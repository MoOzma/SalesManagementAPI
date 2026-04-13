// Services/AuthService.cs
using BCrypt.Net;
using SalesManagementAPI.Data.Repositories;
using SalesManagementAPI.DTOs.Auth;
using SalesManagementAPI.Helpers;

namespace SalesManagementAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepo;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IRepository<User> userRepo, JwtHelper jwtHelper)
        {
            _userRepo = userRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // توحيد الإيميل لحروف صغيرة
            var exists = (await _userRepo.FindAsync(u => u.Email == dto.Email.ToLower())).Any();
            if (exists)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "البريد الإلكتروني مستخدم مسبقاً"
                };

            // تشفير كلمة المرور باستخدام
            // BCrypt
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = hash,
                Role = "SalesRep"
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                FullName = user.FullName,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Message = "تم التسجيل بنجاح"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var users = await _userRepo.FindAsync(u => u.Email == dto.Email.ToLower());
            var user = users.FirstOrDefault();

            // التحقق من وجود المستخدم وتفعيله
            if (user == null || !user.IsActive)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "بيانات الدخول غير صحيحة"
                };

            // التحقق من كلمة المرور
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "بيانات الدخول غير صحيحة"
                };

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                FullName = user.FullName,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Message = "مرحباً " + user.FullName
            };
        }
    }
}