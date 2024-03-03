using Domain.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Persistence.TypeConfigurations;

public class OrderTypeConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.SalesOrderLines).HasField("_salesOrderLines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        // builder.HasMany(e => e.SalesOrderLines).WithOne().IsRequired();

    }
}
