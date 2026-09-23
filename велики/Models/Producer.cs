using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class Producer
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
