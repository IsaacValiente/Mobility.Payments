namespace Mobility.Payments.Data.Repositories
{
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Domain.Entities;
    using System;
    using System.Threading.Tasks;
    using Mobility.Payments.Domain.Repositories;

    /// <summary>
    /// Generic repository for handling CRUD operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this repository will manage.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly ApplicationDbContext context;
        protected readonly ILogger logger;

        public Repository(
            ApplicationDbContext context,
            ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>The added entity.</returns>
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                string errorMessage = $"{nameof(entity)} must not be null. Entity type: {typeof(TEntity).Name}";
                this.logger.LogError(errorMessage);
                throw new ArgumentNullException(nameof(entity), errorMessage);
            }
            await this.context.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Updates an entity in the database.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                string errorMessage = $"{nameof(entity)} must not be null. Entity type: {typeof(TEntity).Name}";
                this.logger.LogError(errorMessage);
                throw new ArgumentNullException(nameof(entity), errorMessage);
            }

            this.context.Update(entity);
            entity.UpdateTrackValues();
            return this.SaveChangesAsync(entity);
        }

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveChangesAsync(TEntity entity)
        {
            if (!this.context.ChangeTracker.HasChanges())
            {
                return;
            }
            await this.context.SaveChangesAsync();
        }
    }
}
