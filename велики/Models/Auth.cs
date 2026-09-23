using System;
using System.Collections.Generic;

namespace велики.Models;

public partial class Auth
{
    public long Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
