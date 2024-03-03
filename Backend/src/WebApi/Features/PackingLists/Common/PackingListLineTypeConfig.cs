using Domain.Entities.PackingListAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Features.PackingLists.Common;

public class PackingListLineTypeConfig : IEntityTypeConfiguration<PackingListLine>
{
    public void Configure(EntityTypeBuilder<PackingListLine> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
    }
}