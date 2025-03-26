using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Menu
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string IconName { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Menuitem> Menuitems { get; set; } = new List<Menuitem>();

    public virtual ICollection<Menurolepermission> Menurolepermissions { get; set; } = new List<Menurolepermission>();
}
