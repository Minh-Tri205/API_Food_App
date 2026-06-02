using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public int UserId { get; set; }

    public string MethodType { get; set; } = null!;

    public string? DisplayName { get; set; }

    public bool? IsDefault { get; set; }

    public virtual User? User { get; set; } = null!;
}
