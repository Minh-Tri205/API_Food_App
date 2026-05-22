using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string OrderCode { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public string DeliveryPhone { get; set; } = null!;

    public decimal? DeliveryFee { get; set; }

    public string? Note { get; set; }

    public int? AddressId { get; set; }

    public string? PaymentMethod { get; set; }

    public int? VoucherId { get; set; }

    public decimal? DiscountAmount { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderTracking> OrderTrackings { get; set; } = new List<OrderTracking>();

    public virtual Review? Review { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
