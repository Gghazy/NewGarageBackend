using Cashif.Application.Abstractions;
namespace Cashif.Infrastructure.Persistence;
public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    private readonly ApplicationDbContext _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
