namespace Mobility.Payments.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Mobility.Payments.Domain.Entities;
    using System;

    /// <summary>
    /// Configures the User entity and its relationships with other entities.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.HasKey(e => e.Username);
            entity.Property(x => x.Username).HasMaxLength(100);
            entity.Property(x => x.PasswordHash).HasMaxLength(256);
            entity.Property(x => x.FirstName).HasMaxLength(50);
            entity.Property(x => x.LastName).HasMaxLength(50);
            entity.Property(x => x.Balance).HasPrecision(19, 4);
        }
    }
}
