using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Routepermission
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Routerolepermission> Routerolepermissions { get; set; } = new List<Routerolepermission>();
}
