using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class User
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public long? AuthId { get; set; }

    public virtual Auth? Auth { get; set; }

    public virtual Role Role { get; set; } = null!;
}
