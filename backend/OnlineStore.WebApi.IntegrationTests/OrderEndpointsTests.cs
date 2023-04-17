using System.Net;
using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using OnlineStore.HttpApiClient;
using OnlineStore.Models.Requests;
using OnlineStore.Models.Responses;

namespace OnlineStore.WebApi.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker = new("ru");

    public OrderEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task Creating_New_Order_happens()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email,
            Name = _faker.Person.UserName,
            Password = _faker.Internet.Password()
        };
        var registerResponse = await client.Register(registerRequest);
        var accountId = Guid.NewGuid();
        // var city = "TestCity";
        // var address = "TestAddress";

        // Act
        var response = await httpClient.PostAsJsonAsync($"/orders/create_order", new PlaceOrderRequest());
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        registerResponse.AccountId.Should().NotBeEmpty();
        order.Should().NotBeNull();
        order!.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNullOrEmpty();
        order.Items.Should().HaveCount(2);
    }
}