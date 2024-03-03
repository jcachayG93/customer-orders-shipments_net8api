using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApi.Tests.TestCommon;

public class CustomApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly Action<IWebHostBuilder> _builderAction;

    public CustomApplicationFactory(
        Action<IWebHostBuilder> builderAction)
    {
        _builderAction = builderAction;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _builderAction(builder);
        base.ConfigureWebHost(builder);
    }
    
   
}