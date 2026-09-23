using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class Measurement
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
