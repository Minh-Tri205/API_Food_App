using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int FoodId { get; set; }

    public int? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public bool? IsSelected { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual FoodItem Food { get; set; } = null!;
}
