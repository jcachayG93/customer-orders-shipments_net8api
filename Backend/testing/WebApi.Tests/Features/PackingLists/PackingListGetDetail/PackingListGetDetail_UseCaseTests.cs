using System.Net.Http.Json;
using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;
using WebApi.Features.PackingLists.Features;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.PackingLists.PackingListGetDetail;

[Collection("Sequential")]
public class PackingListGetDetail_UseCaseTests
: IntegrationTestsBase
{
    private PackingList CreatePackingList()
    {
        PackingList result = new PackingList(EntityIdentity.Random, EntityIdentity.Random);
        
        result.AddLine(EntityIdentity.Random, new("Bolts"), new (10));
        result.AddLine(EntityIdentity.Random, new("Nuts"), new(3));
        
        return result;
    }

    [Fact]
    public async Task CanGetPackingListDetails()
    {
        // ************ ARRANGE ************

        PackingList packingList = CreatePackingList();
        
        CustomApplicationFactory applicationFactory = CreateApplicationFactory();
        
        applicationFactory.AddEntitiesToDatabase(packingList.ToCollection());

        HttpClient client = applicationFactory.CreateClient();

        string endpoint = $"api/packing-lists/{packingList.OrderId}";

        // ************ ACT ************

        PackingListDetailsViewModel? response = await client.GetFromJsonAsync
            <PackingListDetailsViewModel>(endpoint);

        // ************ ASSERT ************

        Assert.NotNull(response);
        Assert.Equal(packingList.OrderId, response.OrderId);

        for (int i = 0; i < packingList.Lines.OrderBy(l=>l.Product).ToArray().Length; i++)
        {
            PackingListLine inputLine = packingList.Lines.OrderBy(l=>l.Product).ToArray()[i];
            PackingListDetailsViewModel.PackingListLineDto resultLine = response.Lines
                .OrderBy(l=>l.ProductName).ToArray()[i];
            
            Assert.Equal(resultLine.ProductName, inputLine.Product);
            Assert.Equal(resultLine.Quantity, inputLine.Quantity);
        }

    }
}