using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.PackingLists.Common;

public class PackingListTypeConfigTests
: TestWithDbContextBase
{
    [Fact]
    public void CanStoreAndRetrieve()
    {
        // ************ ARRANGE ************

        PackingList packingList = new PackingList(
            EntityIdentity.Random, EntityIdentity.Random);
        
        packingList.AddLine(EntityIdentity.Random, new("Bolt"), new(10));
        packingList.AddLine(EntityIdentity.Random, new("Tire"), new(20));
        
        // ************ ACT ************

        AppDbContext db1 = CreateDbContext();
        db1.Add(packingList);
        db1.SaveChanges();

        AppDbContext db2 = CreateDbContext();
        PackingList result = db2.PackingLists.Include(e=>e.Lines).First();
        
        // ************ ASSERT ************

        Assert.Equivalent(packingList, result);
    }
}