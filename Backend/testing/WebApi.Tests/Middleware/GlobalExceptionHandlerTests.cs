using System.Net;
using System.Net.Http.Json;
using Domain.Entities.OrderAggregate;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Exceptions;
using WebApi.Features.Orders.Common;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Middleware;

[Collection("Sequential")]
public class GlobalExceptionHandlerTests
: IntegrationTestsBase
{
    
    [Fact]
    public async Task WhenExceptionOccurs_ReturnsProblemDetails()
    {
        // ************ ARRANGE ************
        
        // Will force the CreateOrder use case to throw an exception
        Mock<ISalesOrdersRepository> repository = new Mock<ISalesOrdersRepository>();
        repository.Setup(x => x.AddAsync(It.IsAny<SalesOrder>()))
            .ThrowsAsync(new EntityNotFoundException("The order does not exist."));
        

        CustomApplicationFactory factory = CreateApplicationFactory(services =>
        {
            services.TestReplaceScopedService<ISalesOrdersRepository, SalesOrderRepository>(
                typeof(SalesOrderRepository), _ => repository.Object);
        });

        HttpClient client = factory.CreateClient();

        string endpoint = "api/sale-orders/" + Guid.NewGuid().ToString();
        
        // ************ ACT ************

        HttpResponseMessage response = await client.PostAsync(endpoint, null);
        
        // ************ ASSERT ************

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        ProblemDetails? result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(result);
        Assert.Equal("The order does not exist.", result.Detail);
        Assert.Equal(404, result.Status);
    }
}