using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Xunit.Abstractions;

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

        var value = await _httpClient.GetFromJsonAsync<IdentityUser>("api/Users");
        
        Assert.True(true);
    }
}