using System.Linq.Expressions;

namespace SalesManagementAPI.Data.Repositories
{
    // T هو متغير النوع (Generic Type) — يمكن أن يكون User أو Order أو أي Model آخر
    public interface IRepository<T> where T : class
    {
        // T? تعني قد تُرجع null إذا لم يُوجد السجل
        Task<T?> GetByIdAsync(int id);

        // جلب كل السجلات — استخدمه بحذر مع الجداول الكبيرة
        Task<IEnumerable<T>> GetAllAsync();

        // بحث بشرط: يُحوَّل لـ WHERE SQL تلقائياً
        // مثال: FindAsync(u => u.Email == email)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // إضافة كيان جديد — لا يُحفظ فعلياً حتى استدعاء SaveChangesAsync
        Task AddAsync(T entity);

        // تعليم الكيان كـ Modified — لا يُحفظ حتى SaveChangesAsync
        void Update(T entity);

        // تعليم الكيان كـ Deleted — لا يُحذف حتى SaveChangesAsync
        void Delete(T entity);

        // ينفّذ كل العمليات المعلّقة في قاعدة البيانات دفعة واحدة
        Task SaveChangesAsync();
    }
}
