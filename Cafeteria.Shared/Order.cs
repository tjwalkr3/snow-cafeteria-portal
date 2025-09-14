namespace Cafeteria.Shared;

public class Order
{
    #region Public Properties
    public int Id { get; init; }
    public string CustomerName { get; init; }
    public string? CustomerEmail { get; init; }
    public DateTime OrderDate { get; init; }
    public OrderStatus Status { get; init; }
    public IReadOnlyList<OrderItem> Items { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
    #endregion

    #region Constructor
    public Order(int id, string customerName, DateTime orderDate, OrderStatus status,
                 IReadOnlyList<OrderItem> items, decimal totalAmount, string? customerEmail = null, string? notes = null)
    {
        Id = id;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        OrderDate = orderDate;
        Status = status;
        Items = items;
        TotalAmount = totalAmount;
        Notes = notes;
    }
    #endregion
}

public class OrderItem
{
    #region Public Properties
    public FoodItem FoodItem { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => UnitPrice * Quantity;
    #endregion

    #region Constructor
    public OrderItem(FoodItem foodItem, int quantity, decimal unitPrice)
    {
        FoodItem = foodItem;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    #endregion
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    Ready,
    Completed,
    Cancelled
}