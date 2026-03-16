
// نستورد ما نحتاجه فقط
using System.Linq.Expressions;

namespace SalesManagementAPI.Data.Repositories
{
    // Interface: عقد (Contract) يحدد العمليات التي يجب على أي Repository تنفيذها
    // T: هو 'متغير نوع' (Generic Type) يمكن أن يمثل أي Model مثل User أو Order
    // where T : class: قيد يضمن أن النوع المستخدم يجب أن يكون Class
    public interface IRepository<T> where T : class
    {
        // GetByIdAsync: عملية غير متزامنة ترجع سجل واحد بواسطة الـ ID أو null
        Task<T?> GetByIdAsync(int id);

        // GetAllAsync: عملية غير متزامنة ترجع مجموعة من كل السجلات
        Task<IEnumerable<T>> GetAllAsync();

        // FindAsync: تتيح لنا تمرير شرط البحث كمعامل (Expression) بمرونة عالية
        // مثال: FindAsync(u => u.Email == "test@test.com")
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // AddAsync: عملية إضافة غير متزامنة لأن التعامل مع قاعدة البيانات يتطلب وقتاً
        Task AddAsync(T entity);

        // Update و Delete: عمليات متزامنة لأنها تعمل على الذاكرة (Memory) أولاً قبل الحفظ
        void Update(T entity);
        void Delete(T entity);

        // SaveChangesAsync: يرسل كل التغييرات المعلقة لقاعدة البيانات دفعة واحدة
        // يرجع true إذا تم حفظ سجل واحد على الأقل بنجاح
        Task<bool> SaveChangesAsync();
    }
}