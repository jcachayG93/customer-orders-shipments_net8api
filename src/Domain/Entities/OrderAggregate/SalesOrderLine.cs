﻿namespace Domain.Entities.OrderAggregate;

/// <summary>
///     A sales order line
/// </summary>
public class SalesOrderLine
{
    // We use an internal constructor so the child entity can't be created by accident from the application layer
    // but using the methods from the aggregate root (Order) instead
    internal SalesOrderLine(
        Guid id, string product, int quantity, decimal unitPrice)
    {
        Id = id;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public string Product { get; private set; }

    public int Quantity { get; }

    public decimal UnitPrice { get; }

    public decimal Total => Quantity * UnitPrice;
}