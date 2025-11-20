using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository
{
    Task<Ue> CreateAsync(Ue entity);
    Task UpdateAsync(Ue entity);
    Task DeleteAsync(long id);
    Task DeleteAsync(Ue entity);
    Task<Ue?> FindAsync(long id);
    Task<Ue?> FindAsync(params object[] keyValues);
    Task<List<Ue>> FindByConditionAsync(Expression<Func<Ue, bool>> condition);
    Task<List<Ue>> FindAllAsync();
    Task SaveChangesAsync();
}