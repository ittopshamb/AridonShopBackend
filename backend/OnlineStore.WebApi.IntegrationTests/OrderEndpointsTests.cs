using Bogus;
using FluentAssertions;
using OnlineStore.HttpApiClient;
using OnlineStore.Models.Requests;
using OnlineStore.Models.Shared;

namespace OnlineStore.WebApi.IntegrationTests;

[Collection("Endpoints")]
public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker = new("ru");

    public OrderEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Place_order_succeeded()
    {
        // Arrange
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email,
            Name = _faker.Person.UserName,
            Password = _faker.Internet.Password()
        };
        var httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerResponse = await client.Register(registerRequest);
        var accountId = registerResponse.AccountId;
        var address = _faker.Person.Address;
        var items = new List<OrderItemDto>()
        {
            new(Guid.NewGuid(), 1, 50m)
        };

        var orderRequest = new PlaceOrderRequest
        {
            OrderId = Guid.NewGuid(),
            AccountId = accountId,
            OrderDate = DateTimeOffset.Now,
            Address = address.Suite + ", " + address.Street,
            City = address.City,
            Items = items
        };

        // Act
        var orderResponse = await client.PlaceOrder(orderRequest);
        
        // Assert
        orderResponse.Should().NotBeNull();
        orderResponse.AccountId.Should().Be(accountId);
        orderResponse.Items.Should().NotBeNull().And.HaveCount(1);
    }
}