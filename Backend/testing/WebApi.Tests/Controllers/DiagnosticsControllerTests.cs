using System.Net.Http.Json;
using WebApi.Tests.TestCommon;

namespace WebApi.Tests.Controllers;

[Collection("Sequential")]
public class DiagnosticsControllerTests : IntegrationTestsBase
{
    private readonly HttpClient _client;

    public DiagnosticsControllerTests()
    {
        CustomApplicationFactory factory = CreateApplicationFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_ReturnsSuccessfulResponse()
    {
        // ************ ARRANGE ************

        string endpoint = "api/diagnostics/ping";

        // ************ ACT ************

        HttpResponseMessage response = await _client.GetAsync(endpoint);
        
        // ************ ASSERT ************

        response.AssertIsSuccessful();
    }
}