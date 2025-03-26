using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Menuitem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string IconName { get; set; } = null!;

    public string Path { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public int MenuId { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual ICollection<Menuitemrolepermission> Menuitemrolepermissions { get; set; } = new List<Menuitemrolepermission>();
}
