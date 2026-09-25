using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity>, IScopedService
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext DbContext;


    public Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }


    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }


    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<TEntity>()
            .ToListAsync(cancellationToken);
    }


    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await DbContext
            .Set<TEntity>()
            .AddAsync(entity, cancellationToken);
    }


    public void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }


    public void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }


    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}