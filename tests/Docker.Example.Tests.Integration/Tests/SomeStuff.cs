using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Identity;

namespace Docker.Example.Tests.Integration.Tests;

public class SomeStuff : IClassFixture<SomethingFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public SomeStuff(SomethingFactory somethingFactory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = somethingFactory.CreateClient();
    }

    [Fact]
    public async Task SomethingElse()
    {
        _testOutputHelper.WriteLine("Hello There");

        var response = await _httpClient.GetAsync("api/Users", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<IdentityUser>(TestContext.Current.CancellationToken);
        user.Should().NotBeNull();
        user!.Id.Should().NotBeNullOrEmpty();
        user.Email.Should().Be("test.test@test.com");
        user.UserName.Should().Be("test.test@test.com");
    }
}
