using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Persistence.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrders");
            builder.HasOne(e=>e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId);
           builder.HasMany(e => e.PurchaseOrderLines)
                .WithOne()
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("PurchaseOrderId");
            builder.Property(e => e.ProcessingDate)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("ProcessingDate");
            builder.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("CustomerId");
              
            builder.Property(e => e.Status)
                  .HasColumnType("int")
                  .HasColumnName("Status")
                  .IsRequired();
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UpdatedAt");
        }
    }
}
