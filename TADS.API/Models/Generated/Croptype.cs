using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Croptype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string GrowingSeason { get; set; } = null!;

    public int? GrowingDays { get; set; }

    public virtual ICollection<Fieldcroprotation> Fieldcroprotations { get; set; } = new List<Fieldcroprotation>();

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
}
