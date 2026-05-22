using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int FoodId { get; set; }

    public int? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual FoodItem Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
