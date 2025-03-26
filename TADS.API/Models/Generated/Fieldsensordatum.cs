using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fieldsensordatum
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string SensorType { get; set; } = null!;

    public decimal Value { get; set; }

    public string Unit { get; set; } = null!;

    public DateTime ReadingTime { get; set; }

    public string SensorId { get; set; } = null!;

    public string Location { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;
}
