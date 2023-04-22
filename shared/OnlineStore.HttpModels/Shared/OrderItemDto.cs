using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Shared;

public class OrderItemDto
{
    public OrderItemDto(Guid productId, int quantity, decimal price)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    [Required] public Guid ProductId { get; init; }
    [Required] public int Quantity { get; init; }
    [Required] public decimal Price { get; init; }
}