using System.Net;
using ECommerceProject.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ECommerceProject.Tests;

public class CheckUserMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CheckUserMiddlewareTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthenticatedUserWithTempUserCookie_RemovesCookieAndCart()
    {
        var tempUserId = Guid.NewGuid().ToString();

        var cartService = _factory.Services.GetRequiredService<FakeCartService>();
        cartService.Clear();
        cartService.SeedTempCart(tempUserId);

        using var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var request = new HttpRequestMessage(HttpMethod.Get, "/User/Login/Index");
        request.Headers.Add("Cookie", $"TempUserId={tempUserId}");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(tempUserId, cartService.TempUserIds);

        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders));
        Assert.Contains(setCookieHeaders, header => header.StartsWith("TempUserId=", StringComparison.OrdinalIgnoreCase));
    }
}
