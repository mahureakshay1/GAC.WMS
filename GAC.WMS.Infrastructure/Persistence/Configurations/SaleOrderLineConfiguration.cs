using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Persistence.Configurations
{
    public class SaleOrderLineConfiguration : IEntityTypeConfiguration<SaleOrderLine>
    {
        public void Configure(EntityTypeBuilder<SaleOrderLine> builder)
        {
            builder.ToTable("SellOrderLines");
            builder.HasOne<Product>()
               .WithMany()
               .HasForeignKey(e => e.ProductId);
           
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("SellOrderLineId");
           
            builder.Property(e => e.Quantity)
                .IsRequired()
                .HasColumnName("Quantity")
                .HasColumnType("int");
            builder.Property(e => e.UnitPrice)
                .IsRequired()
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)");
            builder.Property(e => e.TotalPrice)
                .IsRequired()
                .HasColumnName("TotalPrice")
                .HasColumnType("decimal(18,2)");
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
