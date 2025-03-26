using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fieldhistory
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string ActionType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public string ActionBy { get; set; } = null!;

    public decimal? Cost { get; set; }

    public string Notes { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;
}
