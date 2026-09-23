using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class PickupPoint
{
    public long Id { get; set; }

    public string Index { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
