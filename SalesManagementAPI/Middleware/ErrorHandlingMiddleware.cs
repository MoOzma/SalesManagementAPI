using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // يُستدعى لكل طلب HTTP يمر عبر هذا الـ Middleware
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // نسجّل الخطأ الكامل مع الـ Stack Trace في السجلات (Logs) للمطورين فقط
                _logger.LogError(ex, "Unhandled: {Msg}", ex.Message);
                await WriteErrorAsync(context, ex);
            }
        }

        private static async Task WriteErrorAsync(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";

            // اختيار كود الـ HTTP المناسب بناءً على نوع الاستثناء (Pattern Matching)
            var (code, msg) = ex switch
            {
                // سجل غير موجود في قاعدة البيانات
                KeyNotFoundException => (404, ex.Message),

                // محاولة وصول بدون صلاحية
                UnauthorizedAccessException => (401, "غير مصرح"),

                // معطيات خاطئة من المستخدم
                ArgumentException => (400, ex.Message),

                // عملية غير صالحة منطقياً (مثل تعديل طلب ملغى)
                InvalidOperationException => (422, ex.Message),

                // أي خطأ آخر - لا نكشف تفاصيله الداخلية للعميل أبداً لدواعي أمنية
                _ => (500, "خطأ داخلي، يرجى المحاولة لاحقاً")
            };

            ctx.Response.StatusCode = code;

            await ctx.Response.WriteAsJsonAsync(new
            {
                StatusCode = code,
                Message = msg,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}