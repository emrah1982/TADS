using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Menurolepermission
{
    public int Id { get; set; }

    public int MenuId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;
}
