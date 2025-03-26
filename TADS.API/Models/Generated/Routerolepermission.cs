using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Routerolepermission
{
    public int Id { get; set; }

    public int RoutePermissionId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual Routepermission RoutePermission { get; set; } = null!;
}
