using System.Text;
using Moq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;
using OnlineStore.Domain.Services;

namespace OnlineStore.Domain.Test;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _emailSenderMock = new Mock<IEmailSender>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(uow => uow.CartRepository).Returns(_cartRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow => uow.OrderRepository).Returns(_orderRepositoryMock.Object);

        _orderService = new OrderService(_unitOfWorkMock.Object, _emailSenderMock.Object);
    }

    [Fact]
    public async Task PlaceOrderAndCreateNew_ShouldCreateNewOrderAndSendNotification()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var city = "Test City";
        var address = "Test Address";
        var cancellationToken = CancellationToken.None;

        var cart = new Cart(Guid.Empty, Guid.Empty, new List<CartItem>());
        var product = new Product(Guid.Empty, "fake", 50, "img", "desc", Guid.Empty);
        cart.Add(product, 1);
        var cartItem = cart.Items.First();

        _cartRepositoryMock.Setup(repo => repo.GetByAccountId(accountId, cancellationToken))
                           .ReturnsAsync(cart);

        _orderRepositoryMock.Setup(repo => repo.Add(It.IsAny<Order>(), cancellationToken))
                            .Callback<Order, CancellationToken>((order, ct) =>
                            {
                                // order.Id = Guid.NewGuid();
                            })
                            .Returns(Task.CompletedTask);

        _emailSenderMock.Setup(sender => sender.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                        .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.PlaceOrderAndCreateNew(accountId, city, address, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.AccountId);
        Assert.Single(result.Items);
        Assert.Equal(cartItem.ProductId, result.Items.First().ProductId);
        Assert.Equal(cartItem.Quantity, result.Items.First().Quantity);
        Assert.Equal(cartItem.Price, result.Items.First().Price);
        Assert.Equal(0, cart.Items.Count);
        
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);

        var expectedBody = new StringBuilder()
            .AppendLine("Новый заказ обработан")
            .AppendLine("---")
            .AppendLine("Товары:")
            .AppendFormat("{0} x {1} (Итого: {2:c})",
                cartItem.Price, cartItem.Quantity, cartItem.Price * cartItem.Quantity)
            .AppendFormat("Общая стоимость: {0:c}", result.GetTotalPrice())
            .AppendLine("---")
            .AppendLine("Доставка:")
            .AppendLine(address)
            .AppendLine(city)
            .AppendLine("---")
            .ToString();

        _emailSenderMock.Verify(sender => sender.Send(ShopConfig.ManagerEmail, "Новый заказ отправлен", expectedBody, cancellationToken), Times.Once);
    }
}
