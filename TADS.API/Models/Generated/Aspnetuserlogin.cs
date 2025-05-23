﻿using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Aspnetuserlogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Aspnetuser User { get; set; } = null!;
}
