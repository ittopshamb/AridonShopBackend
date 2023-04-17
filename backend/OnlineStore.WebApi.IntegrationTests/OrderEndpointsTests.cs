using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OnlineStore.Domain.Entities;

namespace OnlineStore.WebApi.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public OrderEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task Creating_New_Order_happens()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var accountId = Guid.NewGuid();
        var city = "TestCity";
        var address = "TestAddress";

        // Act
        var response = await httpClient.PostAsJsonAsync($"/api/orders/create_order", new { city, address });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<Order>();

        // Assert
        order.Should().NotBeNull();
        order!.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNullOrEmpty();
        order.Items.Should().HaveCount(2);
    }
}