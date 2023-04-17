using Moq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;
using OnlineStore.Domain.Services;

namespace OnlineStore.Domain.Test;

public class OrderServiceTests
{
    [Fact]
    public async Task Sending_Email_With_Correct_Parameters_happens()
    {
        // Arrange
        var emailSenderMock = new Mock<IEmailSender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var accountId = Guid.NewGuid();
        var productA = Guid.NewGuid();
        var productB = Guid.NewGuid();
        var productC = Guid.NewGuid();
        var order = new Order(Guid.NewGuid(), accountId, new List<OrderItem>()
        {
            new (Guid.NewGuid(), productA, 2, 10.99m),
            new (Guid.NewGuid(), productB, 3, 5.99m),
            new (Guid.NewGuid(), productC, 1, 19.99m)
        });

        var city = "TestCity";
        var address = "TestAddress";
        var sut = new OrderService(unitOfWorkMock.Object, emailSenderMock.Object);

        // Act
        await sut.NotifyAboutPlacedOrder(order, city, address);

        // Assert
        emailSenderMock.Verify(es => es.Send(ShopConfig.ManagerEmail, "Новый заказ отправлен", It.IsAny<string>(), default), Times.Once);
    }
}
