using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OnlineStore.Domain.Entities;

namespace OnlineStore.WebApi.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public OrderEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Creating_New_Order_happens()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var city = "TestCity";
        var address = "TestAddress";

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/api/orders/place-order/{accountId}", new { city, address });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<Order>();

        // Assert
        order.Should().NotBeNull();
        order!.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNullOrEmpty();
        order.Items.Should().HaveCount(2);
    }
}