using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Field
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Area { get; set; }

    public string Location { get; set; } = null!;

    public int SoilTypeId { get; set; }

    public bool IsActive { get; set; }

    public int? CropTypeId { get; set; }

    public DateOnly? PlantingDate { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual Croptype? CropType { get; set; }

    public virtual ICollection<Fertilization> Fertilizations { get; set; } = new List<Fertilization>();

    public virtual ICollection<Fieldcroprotation> Fieldcroprotations { get; set; } = new List<Fieldcroprotation>();

    public virtual ICollection<Fieldhistory> Fieldhistories { get; set; } = new List<Fieldhistory>();

    public virtual ICollection<Fieldimage> Fieldimages { get; set; } = new List<Fieldimage>();

    public virtual ICollection<Fieldsensordatum> Fieldsensordata { get; set; } = new List<Fieldsensordatum>();

    public virtual ICollection<Irrigation> Irrigations { get; set; } = new List<Irrigation>();

    public virtual Soiltype SoilType { get; set; } = null!;

    public virtual ICollection<Yield> Yields { get; set; } = new List<Yield>();
}
