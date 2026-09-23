using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class Order
{
    public long Id { get; set; }

    public DateOnly DateOfOrdering { get; set; }

    public DateOnly DateOfDelivery { get; set; }

    public long PickupPointId { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Code { get; set; } = null!;

    public long StatusId { get; set; }

    public virtual ICollection<OrderContent> OrderContents { get; set; } = new List<OrderContent>();

    public virtual PickupPoint PickupPoint { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
