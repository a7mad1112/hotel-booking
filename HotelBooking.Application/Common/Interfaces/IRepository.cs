using HotelBooking.Domain.Common;

namespace HotelBooking.Application.Common.Interfaces;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<List<TEntity>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    void Update(
        TEntity entity);

    void Delete(
        TEntity entity);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}