using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Soiltype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
}
