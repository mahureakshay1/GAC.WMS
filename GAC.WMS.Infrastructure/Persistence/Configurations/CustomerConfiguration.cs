using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CustomerId");
            builder.Property(c => c.CompanyName)
                    .HasColumnName("CompanyName")
                    .IsRequired()
                    .HasMaxLength(100);
            builder.Property(c => c.ContactPersonName)
                    .HasColumnName("ContactPersonName")
                    .IsRequired()
                    .HasMaxLength(100);
            builder.Property(c => c.Address)
                    .HasColumnName("Address")
                    .IsRequired()
                    .HasMaxLength(200);
            builder.Property(c => c.Contact)
                   .HasColumnName("Contact")
                   .IsRequired();
            builder.Property(c => c.CreatedAt)
                    .HasColumnName("CreatedAt")
                    .HasColumnType("datetime")
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt)
                    .HasColumnName("UpdatedAt")
                    .HasColumnType("datetime");
        }
    }
}
