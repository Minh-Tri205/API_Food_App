using System;
using System.Collections.Generic;

namespace API_Food_App.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int FoodId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual FoodItem? Food { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
