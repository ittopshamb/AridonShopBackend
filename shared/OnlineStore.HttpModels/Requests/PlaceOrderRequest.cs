using System.ComponentModel.DataAnnotations;
using OnlineStore.Models.Shared;

namespace OnlineStore.Models.Requests;

public class PlaceOrderRequest
{
    [Required] public IReadOnlyList<OrderItemDto> Items { get; set; }
    [Required] public Guid OrderId { get; set; }
    [Required] public Guid AccountId { get; set; }
    [Required] public DateTimeOffset OrderDate { get; set; }
    [Required] public string City { get; set; }
    [Required] public string Address { get; set; }
}