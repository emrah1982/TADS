using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Yield
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public int Season { get; set; }

    public decimal Amount { get; set; }

    public string Unit { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual Field Field { get; set; } = null!;
}
