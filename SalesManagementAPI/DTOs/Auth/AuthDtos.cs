using System.ComponentModel.DataAnnotations;

namespace SalesManagementAPI.DTOs.Auth
{
    //  يرسله المستخدم عند التسجيل
    public class RegisterDto
    {
        [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")] 
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, MinimumLength = 8)] 
        public string Password { get; set; } = string.Empty;
    }

    // ما يُرسله المستخدم عند تسجيل الدخول
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ما يُرجعه
    // API
    // بعد تسجيل الدخول أو التسجيل
    public class AuthResponseDto
    {
        // هل العملية نجحت؟ يُستخدم في الكود لاتخاذ القرار
        public bool Success { get; set; }

        // رسالة للمستخدم: تم التسجيل بنجاح أو بيانات غير صحيحة
        public string Message { get; set; } = string.Empty;

        // Token JWT — موجود عند النجاح، null عند الفشل
        public string? Token { get; set; }

        // اسم المستخدم لعرضه في واجهة التطبيق
        public string? FullName { get; set; }

        // دور المستخدم لتحديد ما يُظهر في واجهة التطبيق
        public string? Role { get; set; }

        // وقت انتهاء التوكن — يُمكّن التطبيق من تجديده قبل الانتهاء
        public DateTime? ExpiresAt { get; set; }
    }
}
