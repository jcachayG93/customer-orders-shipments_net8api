using Domain.Entities.PackingListAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Features.PackingLists.Common;

public class PackingListTypeConfig : IEntityTypeConfiguration<PackingList>
{
    public void Configure(EntityTypeBuilder<PackingList> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.HasMany(e => e.Lines).WithOne();
    }
}