﻿#pragma warning disable CS8618
namespace OnlineStore.Domain.Entities;

public record Product : IEntity
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public decimal Price { get; set; }


    public Product()
    {
    }

    public Product(Guid id, string name, decimal price)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Price = price;
        Id = id;
    }
}