namespace Mobility.Payments.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Mobility.Payments.Domain.Entities;
    using System;

    /// <summary>
    /// Configures the Payment entity and its relationships with other entities.
    /// </summary>
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.HasKey(x => x.Id);
            entity.HasOne(e => e.Sender)
                .WithMany(e => e.Sent)
                .HasForeignKey(e => e.SenderId);

            entity.HasOne(e => e.Receiver)
                .WithMany(e => e.Received)
                .HasForeignKey(e => e.ReceiverId);

            entity.Property(x => x.Amount).HasPrecision(19, 4);
        }
    }
}
