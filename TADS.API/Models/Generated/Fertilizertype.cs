using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fertilizertype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Npk { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Fertilization> Fertilizations { get; set; } = new List<Fertilization>();
}
