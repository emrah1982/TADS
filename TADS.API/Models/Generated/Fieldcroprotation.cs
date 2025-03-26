using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fieldcroprotation
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public int CropTypeId { get; set; }

    public int RotationOrder { get; set; }

    public int Year { get; set; }

    public DateOnly? PlannedPlantingDate { get; set; }

    public DateOnly? PlannedHarvestDate { get; set; }

    public string Notes { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public virtual Croptype CropType { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;
}
