using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string? NotificationType { get; set; }

    public int? RelatedId { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; } = null!;
}
