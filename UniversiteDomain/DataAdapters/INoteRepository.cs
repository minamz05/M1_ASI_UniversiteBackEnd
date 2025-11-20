using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository
{
    Task<Note> CreateAsync(Note entity);
    Task UpdateAsync(Note entity);
    Task DeleteAsync(long id);
    Task DeleteAsync(Note entity);
    Task<Note?> FindAsync(long id);
    Task<Note?> FindAsync(params object[] keyValues);
    Task<List<Note>> FindByConditionAsync(Expression<Func<Note, bool>> condition);
    Task<List<Note>> FindAllAsync();
    Task SaveChangesAsync();
}