﻿using System.Net.Http.Json;
using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;
using WebApi.Features.PackingLists.Features.PackingListGet;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Features.PackingLists.PackingListGet;

[Collection("Sequential")]
public class PackingListGetList_UseCaseTests
    : IntegrationTestsBase
{
  

    private PackingList CreatePackingList()
    {
        return new(EntityIdentity.Random, EntityIdentity.Random);
    }

    [Fact]
    public async Task CanGetListOfPackingLists()
    {
        // ************ ARRANGE ************

        PackingList[] packingLists = Enumerable.Range(0, 10)
            .Select(i => CreatePackingList()).ToArray();

        CustomApplicationFactory applicationFactory = CreateApplicationFactory();
        
        applicationFactory.AddEntitiesToDatabase(packingLists);

        HttpClient client = applicationFactory.CreateClient();

        string endpoint = "api/packing-lists";

        // ************ ACT ************

        IEnumerable<PackingListLookup>? response = await client.GetFromJsonAsync<IEnumerable<PackingListLookup>>
            (endpoint);

        // ************ ASSERT ************

        Assert.NotNull(response);
        Assert.Equal(10, response.Count());

        foreach (PackingListLookup packingListFromResponse in response!)
        {
            Assert.Contains(packingLists, x =>
                x.OrderId == packingListFromResponse.OrderId);
        }

    }
}