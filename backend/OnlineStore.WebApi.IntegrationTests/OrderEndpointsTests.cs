﻿using Bogus;
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
        var items = new List<OrderItemRequest>()
        {
            new()
            {
                ProductId = new Guid(),
                Quantity = 1,
                Price    = 50
            }
        };

        var orderRequest = new PlaceOrderRequest()
        {
            AccountId = accountId,
            Address = address.Suite + ", " + address.Street,
            City = address.City,
            Items = items
        };

        // Act
        var order = await client.PlaceOrder(orderRequest);

        // Assert
        order.Should().NotBeNull();
        order.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNullOrEmpty();
        order.Items.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task Creating_New_Order_with_Invalid_Product_Id_Fails()
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
        var items = new List<OrderItemRequest>()
        {
            new()
            {
                ProductId = Guid.Empty,
                Quantity = 1,
                Price    = 50
            }
        };

        var orderRequest = new PlaceOrderRequest()
        {
            AccountId = accountId,
            Address = address.Suite + ", " + address.Street,
            City = address.City,
            Items = items
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.PlaceOrder(orderRequest));
    }
}