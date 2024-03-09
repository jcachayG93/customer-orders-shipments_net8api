using Domain.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Features.Orders.Common;

public class SalesOrderTypeConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasMany(e => e.SalesOrderLines).WithOne().IsRequired();
        builder.Ignore(e => e.AssertOrderCanChangeWasCalled);
        builder.Ignore(e => e.AssertInvariantsWasCalled);
    }
}