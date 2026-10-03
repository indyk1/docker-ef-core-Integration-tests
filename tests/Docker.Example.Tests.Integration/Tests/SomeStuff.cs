using System.Net;
using System.Text.Json;
using AwesomeAssertions;

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
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        root.ValueKind.Should().Be(JsonValueKind.Object);
        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo("id", "userName", "email");
        root.GetProperty("email").GetString().Should().Be("test.test@test.com");
        root.GetProperty("userName").GetString().Should().Be("test.test@test.com");
        root.GetProperty("id").GetString().Should().NotBeNullOrEmpty();
    }
}
