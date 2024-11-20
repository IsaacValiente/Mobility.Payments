namespace Mobility.Payments.Domain.Repositories
{
    using Mobility.Payments.Domain.Entities;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task SaveChangesAsync(TEntity entity);
    }
}
