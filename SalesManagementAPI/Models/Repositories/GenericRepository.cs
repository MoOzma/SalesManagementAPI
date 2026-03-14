using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SalesManagementAPI.Data.Repositories
{
    // نلتزم بالعقد: تنفيذ الواجهة IRepository لضمان وجود العمليات الأساسية
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        // protected: يمكن للفئات الوارثة الوصول إليه
        // readonly: لا يمكن تغيير قيمته بعد تعيينه في المشيد (Constructor)
        protected readonly AppDbContext _context;

        // _dbSet: يمثل الجدول الفعلي الذي نعمل عليه في قاعدة البيانات
        private readonly DbSet<T> _dbSet;

        // Constructor: يتم حقن سياق قاعدة البيانات (Dependency Injection)
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            // context.Set<T>(): يحدد الجدول بناءً على نوع T (مثلاً لو T هو Order سيفتح جدول Orders)
            _dbSet = context.Set<T>();
        }

        // GetByIdAsync: يبحث عن السجل باستخدام المفتاح الأساسي (Primary Key)
        // يبحث أولاً في الذاكرة (Cache) ثم في قاعدة البيانات
        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        // GetAllAsync: جلب كل السجلات من الجدول
        // تحذير: في الجداول الضخمة يفضل استخدام Pagination بدلاً من جلب الكل
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        // FindAsync: البحث باستخدام شروط Lambda وهو الجزء الأقوى والأكثر استخداماً
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        // AddAsync: يضيف الكيان لـ ChangeTracker الخاص بـ EF Core 
        // يخبر النظام أن هذا الكيان 'جديد' ويجب إدراجه عند الحفظ
        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        // Update: يخبر EF Core أن حالة الكيان أصبحت (Modified)
        public void Update(T entity)
            => _dbSet.Update(entity);

        // Delete: يخبر EF Core أن الكيان سيتم حذفه (Deleted)
        public void Delete(T entity)
            => _dbSet.Remove(entity);

        // SaveChangesAsync: التنفيذ الفعلي لكل العمليات المعلقة في قاعدة البيانات
        // يقوم بتوليد أكواد SQL (INSERT, UPDATE, DELETE) وينفذها دفعة واحدة
        // يرجع true إذا نجح في التأثير على سجل واحد على الأقل
        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}