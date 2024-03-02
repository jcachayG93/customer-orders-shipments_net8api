namespace WebApi.Tests.TestCommon;

public static class HttpResponseMessageExtensions
{
    public static void AssertIsSuccessful(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            Assert.Fail(response.ReasonPhrase ?? "Response was unsuccessful.");   
        }
    }
}