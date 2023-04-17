using Bogus;
using FluentAssertions;
using OnlineStore.HttpApiClient;
using OnlineStore.Models.Requests;

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
        var address = _faker.Person.Address;
        var orderRequest = new PlaceOrderRequest()
        {
            AccountId = Guid.Empty,
            Address = address.Suite + ", " + address.Street,
            City = address.City,
            Items = new List<OrderItemRequest>()
        };

        // Act
        var order = await client.PlaceOrder(orderRequest);

        // Assert
        registerResponse.AccountId.Should().NotBeEmpty();
        order.Should().NotBeNull();
        order.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNullOrEmpty();
        order.Items.Should().HaveCount(2);
    }
}