using OnlineStore.Models.Requests;
using OnlineStore.Models.Shared;

namespace OnlineStore.Models.Responses;

public record OrderResponse(
    IEnumerable<OrderItemDto> Items, Guid OrderId, Guid AccountId, DateTimeOffset OrderDate);