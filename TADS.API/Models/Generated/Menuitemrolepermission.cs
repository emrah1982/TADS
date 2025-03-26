using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Menuitemrolepermission
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual Menuitem MenuItem { get; set; } = null!;
}
