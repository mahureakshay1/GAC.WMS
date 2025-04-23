using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductId");
            builder.Property(p => p.Code)
                .HasColumnName("Code")
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(p => p.Length)
                .HasColumnName("Length")
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Width)
                .HasColumnName("Width")
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Height)
                .HasColumnName("Height")
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Weight)
                .HasColumnName("Weight")
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.UnitOfDimension)
                .HasColumnName("UnitOfDimension")
                .IsRequired()
                .HasColumnType("int");
            builder.Property(p => p.QuantityAvailable)
                .HasColumnName("QuantityAvailable")
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.UnitOfQuantity)
                .HasColumnName("UnitOfQuantity")
                .IsRequired()
                .HasColumnType("int");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime");

        }
    }
}
