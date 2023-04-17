using Moq;
using Bogus;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;
using OnlineStore.Domain.Services;

namespace OnlineStore.WebApi.IntegrationTests;

public class OrderEndpointsTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly OrderService _orderService;
    private readonly Faker _faker;

    public OrderEndpointsTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _emailSenderMock = new Mock<IEmailSender>();
        _orderService = new OrderService(_unitOfWorkMock.Object, _emailSenderMock.Object);
        _faker = new Faker();
    }

    [Fact]
    public async Task PlaceOrderAndCreateNew_ShouldCreateOrderAndNotifyManager()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var city = _faker.Address.City();
        var address = _faker.Address.FullAddress();
        var productA = Guid.NewGuid();
        var productB = Guid.NewGuid();
        var productC = Guid.NewGuid();

        var cart = new Cart(Guid.Empty, accountId, new List<CartItem>()
        {
            new CartItem(Guid.NewGuid(), productA, 2, 10.99m),
            new CartItem(Guid.NewGuid(), productB, 3, 5.99m),
            new CartItem(Guid.NewGuid(), productC, 1, 19.99m)
        });

        var order = new Order(Guid.NewGuid(), accountId, new List<OrderItem>()
        {
            new OrderItem(Guid.NewGuid(), productA, 2, 10.99m),
            new OrderItem(Guid.NewGuid(), productB, 3, 5.99m),
            new OrderItem(Guid.NewGuid(), productC, 1, 19.99m)
        });

        _unitOfWorkMock.Setup(uow => uow.CartRepository.GetByAccountId(accountId, default))
            .ReturnsAsync(cart);

        _unitOfWorkMock.Setup(uow => uow.OrderRepository.Add(order, default))
            .Returns(Task.CompletedTask);

        _emailSenderMock.Setup(es => es.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.PlaceOrderAndCreateNew(accountId, city, address, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.CartRepository.GetByAccountId(accountId, default), Times.Once);
        // _unitOfWorkMock.Verify(uow => uow.OrderRepository.Add(order, default), Times.Once);
        _emailSenderMock.Verify(es => es.Send(ShopConfig.ManagerEmail, "Новый заказ отправлен", It.IsAny<string>(), default), Times.Once);

        // Assert.Equal(order.Id, result.Id);
        Assert.Equal(accountId, result.AccountId);
        Assert.Equal(order.Items.Count, result.Items.Count);
        Assert.Equal(order.GetTotalPrice(), result.GetTotalPrice());
    }
}


