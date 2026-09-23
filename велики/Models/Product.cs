using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class Product
{
    public long Id { get; set; }

    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long MeasurementId { get; set; }

    public decimal Cost { get; set; }

    public long SupplierId { get; set; }

    public long ProducerId { get; set; }

    public long CategoryId { get; set; }

    public long Discount { get; set; }

    public long Amount { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Measurement Measurement { get; set; } = null!;

    public virtual Producer Producer { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
