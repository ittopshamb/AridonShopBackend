using Moq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;
using OnlineStore.Domain.Services;

namespace OnlineStore.Domain.Test;

public class OrderServiceTests
{
    [Fact]
    public async Task Email_is_sent_when_order_placed()
    {
        // Arrange
        var emailSenderMock = new Mock<IEmailSender>();
        var uow = new Mock<IUnitOfWork>();
        var accountId = Guid.NewGuid();
        uow.Setup(x => x.CartRepository.GetByAccountId(accountId, default))
            .ReturnsAsync(new Cart(Guid.NewGuid(), accountId, new List<CartItem>()));
        uow.Setup(x => x.OrderRepository.Add(It.IsAny<Order>(), default))
            .Returns(Task.CompletedTask);
        var productA = Guid.NewGuid();
        var productB = Guid.NewGuid();
        var productC = Guid.NewGuid();
        var order = new Order(Guid.NewGuid(), accountId, "Kaluga", "ул. Счастливая 33",
            new List<OrderItem>()
        {
            new (Guid.NewGuid(), productA, 2, 10.99m),
            new (Guid.NewGuid(), productB, 3, 5.99m),
            new (Guid.NewGuid(), productC, 1, 19.99m)
        });

        var sut = new OrderService(uow.Object, emailSenderMock.Object);
        var city = "TestCity";
        var address = "TestAddress";

        // Act
        await sut.PlaceOrderAndCreateNew(accountId, city, address, order.Items, default);

        // Assert
        emailSenderMock.Verify(es => es.Send(ShopConfig.ManagerEmail, "Новый заказ отправлен", It.IsAny<string>(), default), Times.Once);
    }
}
