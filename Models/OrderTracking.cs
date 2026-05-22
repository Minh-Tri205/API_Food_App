using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class OrderTracking
{
    public int TrackingId { get; set; }

    public int OrderId { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
