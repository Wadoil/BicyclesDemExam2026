using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class OrderContent
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public long Amount { get; set; }

    public virtual Order Order { get; set; } = null!;
}
