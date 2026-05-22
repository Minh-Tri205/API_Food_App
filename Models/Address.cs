using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int UserId { get; set; }

    public string? Label { get; set; }

    public string FullAddress { get; set; } = null!;

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public bool? IsDefault { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
