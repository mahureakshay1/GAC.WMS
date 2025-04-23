using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Persistence.Configurations
{
    public class SaleOrderConfiguration : IEntityTypeConfiguration<SaleOrder>
    {
        public void Configure(EntityTypeBuilder<SaleOrder> builder)
        {
            builder.ToTable("SaleOrders");
            builder.HasOne(e => e.Customer)
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId);
            builder.HasMany(e => e.SaleOrderLines)
                   .WithOne()
                   .HasForeignKey(e => e.SaleOrderId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("SaleOrderId");
            builder.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("CustomerId");
            builder.Property(e => e.ProcessingDate)
                .IsRequired()
                .HasColumnName("ProcessingDate")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.Status)
                .IsRequired()
                .HasColumnName("Status")
                .HasMaxLength(50);
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime");
        }
    }
}
